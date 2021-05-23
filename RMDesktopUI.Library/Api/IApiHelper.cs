using RMDesktopUI.Models;
using System.Threading.Tasks;
using RMDesktopUI.Library.Models;


namespace RMDesktopUI.Library.Api
{
    public interface IApiHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);

        Task GetLoggedInUserInfo(string token);
    }
}
