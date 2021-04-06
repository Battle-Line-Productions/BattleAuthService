namespace BattleAuth.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using Contracts.Domain.V1;
    using Contracts.Options;
    using Extensions;
    using Interfaces;
    using Microsoft.IdentityModel.Tokens;

    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;


        public JwtService(JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public IList<Claim> BuildClaims(IList<Claim> claims, User user, DateTime now, long expiresAt)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, expiresAt.ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim("id", user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

            return claims;
        }

        public JwtResponse CreateToken(IList<Claim> claims, DateTime now, long expiresAt)
        {
            var privateKey = _jwtSettings.Private.ToByteArray();

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);

            var signingCredentials =
                new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory() { CacheSignatureProviders = false }
                };

            var jwt = new JwtSecurityToken(
                audience: _jwtSettings.Audience,
                issuer: _jwtSettings.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(60),
                signingCredentials: signingCredentials);


            return new JwtResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                ExpiresAt = expiresAt,
                Id = jwt.Id
            };
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                return !IsJwtWithValidSecurityAlgorithm(validatedToken) ? null : principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
