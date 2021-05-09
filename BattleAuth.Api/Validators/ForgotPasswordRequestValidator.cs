namespace BattleAuth.Api.Validators
{
    using Contracts.Contracts.V1.Requests;
    using FluentValidation;

    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}