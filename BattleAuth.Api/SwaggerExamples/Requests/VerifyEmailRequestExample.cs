namespace BattleAuth.Api.SwaggerExamples.Requests
{
    using Contracts.Contracts.V1.Requests;
    using Swashbuckle.AspNetCore.Filters;

    public class VerifyEmailRequestExample : IExamplesProvider<VerifyEmailRequest>
    {
        public VerifyEmailRequest GetExamples()
        {
            return new VerifyEmailRequest()
            {
                code = "Some really long code here",
                Email = "Email@Email.com"
            };
        }
    }
}