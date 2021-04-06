namespace BattleAuth.Contracts.Domain.V1
{
    public class JwtCustomClaims
    {
        public string Email { get; set; }

        public string Id { get; set; }

        public string Role { get; set; }
    }
}