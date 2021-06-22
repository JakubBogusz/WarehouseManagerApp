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
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sqlDataAccess;

        public InventoryData(IConfiguration config, ISqlDataAccess sqlDataAccess)
        {
            _config = config;
            _sqlDataAccess = sqlDataAccess;
        }

        public List<InventoryModel> GetInventory()
        {
            var output = 
                _sqlDataAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "WarehouseManagerData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            _sqlDataAccess.SaveData("dbo.spInventory_Insert", item, "WarehouseManagerData");
        }
    }
}
