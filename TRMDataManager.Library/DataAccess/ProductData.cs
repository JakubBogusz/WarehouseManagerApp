using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ProductData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public List<ProductModel> GetProducts()
        {
            var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "WarehouseManagerData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spGetById", new { Id = productId }, "WarehouseManagerData").FirstOrDefault();

            return output;
        }
    }
}
