namespace BattleAuth.Api.Controllers.V1
{
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Contracts.V1;
    using Contracts.Contracts.V1.Requests;
    using Contracts.Contracts.V1.Response;
    using Contracts.Domain.V1;
    using Microsoft.AspNetCore.Mvc;
    using Service.Interfaces;

    [Produces("application/json")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Allows a user to register for an account. Default role of User.
        /// </summary>
        /// <remarks>Requires valid email address and account verification after registration.</remarks>
        /// <response code="201">Allows a user to register for an account. Default role of User.</response>
        /// <response code="400">Unable to register due to validation error.</response>
        [HttpPost(ApiRoutes.Identity.Register)]
        [ProducesResponseType(typeof(Response<AccountCreationResponse>), 201)]
        [ProducesResponseType(typeof(Response<AccountCreationFailedResponse>), 400)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.GetActionResult(ResultStatus.Error400,
                    new Response<AccountCreationFailedResponse>()
                    {
                        Data = new AccountCreationFailedResponse()
                        {
                            Success = false,
                            Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                        }
                    });

            }

            var authResponse = await _identityService.Register(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return ApiResponse.GetActionResult(ResultStatus.Error400,
                    new Response<AccountCreationFailedResponse>()
                    {
                        Data = new AccountCreationFailedResponse()
                        {
                            Success = false,
                            Errors = authResponse.Errors
                        }
                    });
            }

            return ApiResponse.GetActionResult(ResultStatus.Created201,
                new Response<AccountCreationResponse>()
                {
                    Data = new AccountCreationResponse()
                    {
                        Success = true,
                        Message =
                            "Your account has been successfully created. Please check your email to confirm your account."
                    }
                });
        }

        /// <summary>
        /// Logs a user in. Returns a JWT and Refresh Token.
        /// </summary>
        /// <response code="200">Logs a user in. Returns a JWT and Refresh Token.</response>
        /// <response code="401">User not found</response>
        [HttpPost(ApiRoutes.Identity.Login)]
        [ProducesResponseType(typeof(Response<AuthSuccessResponse>), 200)]
        [ProducesResponseType(typeof(Response<AuthFailedResponse>), 401)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.Login(request.Email, request.Password);

            if (!authResponse.Success)
            {

                return ApiResponse.GetActionResult(ResultStatus.Error401,
                    new Response<AuthFailedResponse>()
                    {
                        Data = new AuthFailedResponse()
                        {
                            Errors = authResponse.Errors
                        }
                    });
            }

            return ApiResponse.GetActionResult(ResultStatus.Ok200,
                new Response<AuthSuccessResponse>()
                {
                    Data = new AuthSuccessResponse()
                    {
                        Token = authResponse.Token,
                        RefreshToken = authResponse.RefreshToken,
                        ExpiresAt = authResponse.ExpiresAt
                    }
                });
        }

        /// <summary>
        /// Refreshes a JWT Token. Will return a new JWT and a new Refresh token when a Expired JWT and Active Refresh token is provided.
        /// </summary>
        /// <response code="200">Refreshes a JWT Token</response>
        /// <response code="401">JWT not found</response>
        [HttpPost(ApiRoutes.Identity.Refresh)]
        [ProducesResponseType(typeof(Response<AuthSuccessResponse>), 200)]
        [ProducesResponseType(typeof(Response<AuthFailedResponse>), 401)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.Refresh(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return ApiResponse.GetActionResult(ResultStatus.Error401,
                    new Response<AuthFailedResponse>()
                    {
                        Data = new AuthFailedResponse()
                        {
                            Errors = authResponse.Errors
                        }
                    });
            }

            return ApiResponse.GetActionResult(ResultStatus.Ok200,
                new Response<AuthSuccessResponse>()
                {
                    Data = new AuthSuccessResponse()
                    {
                        Token = authResponse.Token,
                        RefreshToken = authResponse.RefreshToken,
                        ExpiresAt = authResponse.ExpiresAt
                    }
                });
        }

        /// <summary>
        /// Is used to verify a users email is a valid email after registering an account
        /// </summary>
        /// <response code="200">Successfully verified users account</response>
        /// <response code="400">User not found</response>
        [HttpPost(ApiRoutes.Identity.VerifyEmail)]
        [ProducesResponseType(typeof(Response<VerifyEmailResponse>), 200)]
        [ProducesResponseType(typeof(Response<VerifyEmailFailResponse>), 400)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var verifyResponse = await _identityService.VerifyEmail(request.Email, request.code);

            if (!verifyResponse.Success)
            {
                return ApiResponse.GetActionResult(ResultStatus.Error400,
                    new Response<VerifyEmailFailResponse>()
                    {
                        Data = new VerifyEmailFailResponse()
                        {
                            Errors = verifyResponse.Errors
                        }
                    });
            }

            return ApiResponse.GetActionResult(ResultStatus.Ok200,
                new Response<VerifyEmailResponse>()
                {
                    Data = new VerifyEmailResponse()
                    {
                        Message =
                            "Your account has been successfully Verified. You are now able to log in."
                    }
                });
        }
    }
}
