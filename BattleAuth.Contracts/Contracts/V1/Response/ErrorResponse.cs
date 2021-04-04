namespace BattleAuth.Contracts.Contracts.V1.Response
{
    using System.Collections.Generic;

    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    }
}
