using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.Library.Models
{
    public class SaleModel
    {
        public List<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}