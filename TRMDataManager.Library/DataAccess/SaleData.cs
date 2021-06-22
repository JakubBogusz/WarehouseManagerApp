using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sqlDataAccess;

        public SaleData(IProductData productData, ISqlDataAccess sqlDataAccess)
        {
            _productData = productData;
            _sqlDataAccess = sqlDataAccess;
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO - Make this SOLID/DRY
            // Start filling in the saleInfo detail models we will save to the database

            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();

            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = _productData.GetProductById(item.ProductId);

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

            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = saleDetails.Sum(x => x.PurchasePrice),
                Tax = saleDetails.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;


            try
            {
                _sqlDataAccess.StartTransaction("WarehouseManagerData");
                _sqlDataAccess.SaveDataInTransaction("dbo.spSale_Insert", sale);

                //Get the ID from the saleInfo model
                sale.Id = _sqlDataAccess.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new
                {
                    sale.CashierId,
                    sale.SaleDate
                }).FirstOrDefault();


                //Finish filling the saleInfo detail  models
                foreach (var item in saleDetails)
                {
                    item.SaleId = sale.Id;

                    _sqlDataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sqlDataAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                _sqlDataAccess.RollbackTransaction();
                throw;
            }

        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output =
                _sqlDataAccess.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "WarehouseManagerData");

            return output;
        }
    }
}
