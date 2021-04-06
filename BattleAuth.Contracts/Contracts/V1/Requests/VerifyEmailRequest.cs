namespace BattleAuth.Contracts.Contracts.V1.Requests
{
    public class VerifyEmailRequest
    {
        public string Email { get; set; }

        public string code { get; set; }
    }
}