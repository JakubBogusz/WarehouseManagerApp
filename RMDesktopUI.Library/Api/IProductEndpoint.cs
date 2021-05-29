using System.Collections.Generic;
using System.Threading.Tasks;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<UIProductModel>> GetAll();
    }
}