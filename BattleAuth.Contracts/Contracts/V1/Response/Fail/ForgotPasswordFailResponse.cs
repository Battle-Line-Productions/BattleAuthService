namespace BattleAuth.Contracts.Contracts.V1.Response.Fail
{
    using System.Collections.Generic;

    public class ForgotPasswordFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}