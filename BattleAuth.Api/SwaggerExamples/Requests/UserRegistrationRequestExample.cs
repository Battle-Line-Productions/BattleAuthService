namespace BattleAuth.Api.SwaggerExamples.Requests
{
    using Contracts.Contracts.V1.Requests;
    using Swashbuckle.AspNetCore.Filters;

    public class UserRegistrationRequestExample : IExamplesProvider<UserRegistrationRequest>
    {
        public UserRegistrationRequest GetExamples()
        {
            return new UserRegistrationRequest
            {
                Email = "Email@Email.com",
                Password = "Tgh765%$#Mhg",
                PasswordConfirmation = "Tgh765%$#Mhg"
            };
        }
    }
}
