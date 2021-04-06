namespace BattleAuth.Api.Validators
{
    using Contracts.Contracts.V1.Requests;
    using FluentValidation;

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}