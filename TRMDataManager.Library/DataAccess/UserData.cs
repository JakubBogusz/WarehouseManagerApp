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
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public UserData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public List<UserModel> GetUserById(string userId)
        {
            var p = new {Id = userId};

            var output = _sqlDataAccess.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "WarehouseManagerData");

            return output;
        }

        public void CreateUser(UserModel user)
        {
            _sqlDataAccess.SaveData("dbo.spUser_Insert", 
                new { user.Id, user.FirstName, user.LastName, user.EmailAddress }, "WarehouseManagerData");
        }
    }
}
