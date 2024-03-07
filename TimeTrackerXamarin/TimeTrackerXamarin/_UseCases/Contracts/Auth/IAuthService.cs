using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Auth.Dto;

namespace TimeTrackerXamarin._UseCases.Contracts.Auth
{
    public interface IAuthService
    {
        Task Login(LoginDto data);
        Task Logout();
        Task<User> Current();
        Task<bool> CheckLogin();
        Task<string> SaveAvatar(string avatar);
    }
}