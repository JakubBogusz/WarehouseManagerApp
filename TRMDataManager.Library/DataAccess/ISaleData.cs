using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface ISaleData
    {
        void SaveSale(SaleModel saleInfo, string cashierId);
        decimal GetTaxRate();
        List<SaleReportModel> GetSaleReport();

    }
}