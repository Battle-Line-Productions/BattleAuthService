namespace BattleAuth.Service.Interfaces
{
    using System.Threading.Tasks;
    using Contracts.Domain.V1;

    public interface IIdentityService
    {
        Task<AuthenticationResult> Register(string email, string password);

        Task<AuthenticationResult> Login(string email, string password);

        Task<AuthenticationResult> Refresh(string token, string refreshToken);
    }
}
