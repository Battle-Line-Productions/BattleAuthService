namespace BattleAuth.Contracts.Domain.V1
{
    using System.Collections.Generic;

    public class AccountCreationResult
    {
        public string EmailConfirmationCode { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public bool Success { get; set; }
    }
}