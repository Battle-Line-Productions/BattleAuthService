namespace BattleAuth.Contracts.Contracts.V1.Response.Fail
{
    using System.Collections.Generic;

    public class ResetPasswordFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}