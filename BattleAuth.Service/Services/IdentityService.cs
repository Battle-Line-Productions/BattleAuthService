namespace BattleAuth.Service.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Domain.V1;
    using Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public IdentityService(UserManager<User> userManager, ApplicationDbContext context, IJwtService jwtService)
        {
            _userManager = userManager;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthenticationResult> Register(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
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
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUser(newUser);
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

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
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
