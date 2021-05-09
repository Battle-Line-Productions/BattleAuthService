namespace BattleAuth.Service.Interfaces
{
    using System.Threading.Tasks;
    using Contracts.Domain.V1;

    public interface IIdentityService
    {
        Task<AccountCreationResult> Register(string email, string password);

        Task<AuthenticationResult> Login(string email, string password);

        Task<AuthenticationResult> Refresh(string token, string refreshToken);

        Task<VerifyEmailResult> VerifyEmail(string email, string code);

        Task<ForgotPasswordResult> ForgotPassword(string email);

        Task<ResetPasswordResult> ResetPassword(string email, string code, string newPassword);
    }
}
