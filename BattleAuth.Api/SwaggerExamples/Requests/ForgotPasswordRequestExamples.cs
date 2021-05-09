namespace BattleAuth.Api.SwaggerExamples.Requests
{
    using Contracts.Contracts.V1.Requests;
    using Swashbuckle.AspNetCore.Filters;

    public class ForgotPasswordRequestExamples : IExamplesProvider<ForgotPasswordRequest>
    {
        public ForgotPasswordRequest GetExamples()
        {
            return new ForgotPasswordRequest()
            {
                Email = "Email@email.com"
            };
        }
    }
}