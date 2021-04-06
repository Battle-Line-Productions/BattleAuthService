namespace BattleAuth.Contracts.Domain.V1
{
    using System.Collections.Generic;

    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public long ExpiresAt { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
