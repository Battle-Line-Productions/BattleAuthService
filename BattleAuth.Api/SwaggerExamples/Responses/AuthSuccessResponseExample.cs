namespace BattleAuth.Api.SwaggerExamples.Responses
{
    using Contracts.Contracts.V1.Response;
    using Swashbuckle.AspNetCore.Filters;

    public class AuthSuccessResponseExample : IExamplesProvider<AuthSuccessResponse>
    {
        public AuthSuccessResponse GetExamples()
        {
            return new AuthSuccessResponse
            {
                RefreshToken = "Single user refresh token GUID here",
                Token = "Big long token here"
            };
        }
    }
}