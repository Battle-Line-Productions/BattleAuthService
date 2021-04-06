namespace BattleAuth.Contracts.Domain.V1
{
    using System.IdentityModel.Tokens.Jwt;

    public class JwtResponse
    {
        public JwtSecurityToken Token { get; set; }

        public long ExpiresAt { get; set; }
    }
}