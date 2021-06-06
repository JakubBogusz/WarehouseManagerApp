using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO - Make this SOLID/DRY
            // Start filling in the saleInfo detail models we will save to the database

            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = products.GetProductById(item.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {item.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }
                saleDetails.Add(detail);
            }
 
            // Create saleInfo model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = saleDetails.Sum(x => x.PurchasePrice),
                Tax = saleDetails.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            //Save saleInfo model
            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", sale, "WarehouseManagerData");

            //Get the ID from the saleInfo model
            //Finish filling the saleInfo detail  models
            foreach (var item in saleDetails)
            {
                item.SaleId = sale.Id;

                sql.SaveData("dbo.spSaleDetail_Insert", item, "WarehouseManagerData");
            }

            
            
            //Save the saleInfo detail models

        }
        //public List<ProductModel> GetProducts()
        //{
        //    SqlDataAccess sql = new SqlDataAccess();

        //    var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "WarehouseManagerData");

        //    return output;
        //}
    }
}
