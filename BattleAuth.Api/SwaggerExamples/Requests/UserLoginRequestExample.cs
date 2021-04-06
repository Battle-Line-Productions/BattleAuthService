namespace BattleAuth.Api.SwaggerExamples.Requests
{
    using Contracts.Contracts.V1.Requests;
    using Swashbuckle.AspNetCore.Filters;

    public class UserLoginRequestExample : IExamplesProvider<UserLoginRequest>
    {
        public UserLoginRequest GetExamples()
        {
            return new UserLoginRequest()
            {
                Email = "Email@Email.com",
                Password = "Tgh765%$#Mhg"
            };
        }
    }
}
