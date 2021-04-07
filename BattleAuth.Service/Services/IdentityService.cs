namespace BattleAuth.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using BattleNotifications.Contracts.Contracts.V1.Requests;
    using BattleNotifications.Contracts.Domain.V1;
    using BattleNotifications.Sdk;
    using Contracts.Domain.V1;
    using Contracts.Options;
    using Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Refit;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly NotificationSettings _notificationSettings;

        public IdentityService(UserManager<User> userManager, ApplicationDbContext context, IJwtService jwtService, SignInManager<User> signInManager, NotificationSettings notificationSettings)
        {
            _userManager = userManager;
            _context = context;
            _jwtService = jwtService;
            _signInManager = signInManager;
            _notificationSettings = notificationSettings;
        }

        public async Task<AccountCreationResult> Register(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AccountCreationResult
                {
                    Success = false,
                    Errors = new[] { "User with this email address already exists" }
                };
            }

            var newUser = new User()
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);
            await _userManager.AddToRoleAsync(newUser, "User");

            if (!createdUser.Succeeded)
            {
                return new AccountCreationResult
                {
                    Success = false,
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var notificationsApi = RestService.For<INotificationApi>(_notificationSettings.Url);

            var newUserEmailResponse = await notificationsApi.SendEmailAsync(new EmailSendRequest()
            {
                To = newUser.Email,
                From = "noreply@battlelineproductions.com",
                TemplateChoice = EmailTemplateChoices.ConfirmAccount,
                TemplateData = new List<KeyValueTemplatePairs>()
                {
                    new KeyValueTemplatePairs()
                    {
                        Key = "Code",
                        Value = emailConfirmationCode
                    }
                }
            }, _notificationSettings.ApiKey);

            if (!newUserEmailResponse.IsSuccessStatusCode)
            {
                return new AccountCreationResult
                {
                    Success = false,
                    Errors = new []{ "Failed to send account confirmation email. You will not be able to log in without this and should try to create account again." }
                };
            }

            return new AccountCreationResult()
            {
                Success = true,
                EmailConfirmationCode = emailConfirmationCode
            };
        }

        public async Task<AuthenticationResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist." }
                };
            }

            var userHasValidPassword = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!userHasValidPassword.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist." }
                };
            }

            return await GenerateAuthenticationResultForUser(user);
        }

        public async Task<AuthenticationResult> Refresh(string token, string refreshToken)
        {
            var validatedToken = _jwtService.GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "Token is not valid." } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUser(user);
        }

        public async Task<VerifyEmailResult> VerifyEmail(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new VerifyEmailResult()
                {
                    Success = false,
                    Errors = new[] {"User does not exist"}
                };
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return new VerifyEmailResult()
                {
                    Success = false,
                    Errors = result.Errors.Select(x => x.Description)
                };
            }

            var notificationsApi = RestService.For<INotificationApi>(_notificationSettings.Url);

            // If this failed we are not going to stop account verification and will later log an error somewhere for an admin to work through.
            await notificationsApi.SendEmailAsync(new EmailSendRequest()
            {
                To = user.Email,
                From = "noreply@battlelineproductions.com",
                TemplateChoice = EmailTemplateChoices.NewAccount
            }, _notificationSettings.ApiKey);

            return new VerifyEmailResult()
            {
                Success = true
            };
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(User user)
        {
            var now = DateTime.UtcNow;
            var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

            var userClaims = await _userManager.GetClaimsAsync(user);

            var jwtClaims = _jwtService.BuildClaims(userClaims, user, now, unixTimeSeconds);

            var jwtSecurityToken = _jwtService.CreateToken(jwtClaims, now, unixTimeSeconds);

            var refreshToken = new RefreshToken
            {
                JwtId = jwtSecurityToken.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                Token = Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = jwtSecurityToken.Token,
                ExpiresAt = unixTimeSeconds,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
