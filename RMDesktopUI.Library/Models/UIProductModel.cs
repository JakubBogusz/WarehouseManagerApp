using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Models
{
    public class UIProductModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string RetailPrice { get; set; }

        public string QuantityInStock { get; set; }
    }
}
