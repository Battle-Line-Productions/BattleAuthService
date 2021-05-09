namespace BattleAuth.Contracts.Contracts.V1.Response.Success
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public long ExpiresAt { get; set; }
    }
}
