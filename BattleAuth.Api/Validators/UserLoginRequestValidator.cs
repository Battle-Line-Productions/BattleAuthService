namespace BattleAuth.Api.Validators
{
    using Contracts.Contracts.V1.Requests;
    using FluentValidation;

    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(128).MinimumLength(8).Equal(y => y.PasswordConfirmation);
        }
    }
}
