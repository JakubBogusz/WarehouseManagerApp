using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface ISaleData
    {
        void SaveSale(SaleModel saleInfo, string cashierId);
        List<SaleReportModel> GetSaleReport();
    }
}