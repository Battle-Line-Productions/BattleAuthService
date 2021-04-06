namespace BattleAuth.Contracts.Contracts.V1.Response
{
    using System.Collections.Generic;

    public class VerifyEmailFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}