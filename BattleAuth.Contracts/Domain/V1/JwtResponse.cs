namespace BattleAuth.Contracts.Domain.V1
{
    public class JwtResponse
    {
        public string Token { get; set; }

        public long ExpiresAt { get; set; }

        public string Id { get; set; }
    }
}