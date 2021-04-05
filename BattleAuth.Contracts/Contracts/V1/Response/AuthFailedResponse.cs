namespace BattleAuth.Contracts.Contracts.V1.Response
{
    using System.Collections.Generic;

    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }

    }
}
