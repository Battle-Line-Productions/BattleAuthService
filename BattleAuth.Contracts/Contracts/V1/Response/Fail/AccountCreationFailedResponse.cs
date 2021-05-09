namespace BattleAuth.Contracts.Contracts.V1.Response.Fail
{
    using System.Collections.Generic;

    public class AccountCreationFailedResponse
    {
        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}