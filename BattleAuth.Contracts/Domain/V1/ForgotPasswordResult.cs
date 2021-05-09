namespace BattleAuth.Contracts.Domain.V1
{
    using System.Collections.Generic;

    public class ForgotPasswordResult
    {
        public bool Success { get; set; }
        
        public IEnumerable<string> Errors { get; set; }
    }
}