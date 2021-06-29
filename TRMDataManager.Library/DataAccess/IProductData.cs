using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        List<ProductModel> GetProducts();

        ProductModel GetProductById(int productId);
    }
}