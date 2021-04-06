namespace BattleAuth.Api.Validators
{
    using Contracts.Contracts.V1.Requests;
    using FluentValidation;

    public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.code).NotEmpty();
        }
    }
}