namespace BattleAuth.Api.SwaggerExamples.Requests
{
    using System;
    using Contracts.Contracts.V1.Requests;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Swashbuckle.AspNetCore.Filters;

    public class RefreshTokenRequestExample: IExamplesProvider<RefreshTokenRequest>
    {
        public RefreshTokenRequest GetExamples()
        {
            return new RefreshTokenRequest()
            {
                RefreshToken = Guid.NewGuid().ToString(),
                Token = "JWT token here"
            };
        }
    }
}