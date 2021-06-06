using System.Threading.Tasks;
using RMDesktopUI.Library.Models;
using TRMDataManager.Models;

//using TRMDataManager.Models;

namespace RMDesktopUI.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}