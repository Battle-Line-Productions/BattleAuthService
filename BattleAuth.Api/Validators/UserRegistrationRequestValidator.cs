namespace BattleAuth.Api.Validators
{
    using Contracts.Contracts.V1.Requests;
    using FluentValidation;

    public class UserRegistrationRequestValidator: AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must be provided.").MinimumLength(8).MaximumLength(128).Equal(y => y.PasswordConfirmation)
                .When(x => !string.IsNullOrWhiteSpace(x.Password)).WithMessage("Password and Confirm Password must match.");
        }
    }
}