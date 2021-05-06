using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Launchpad.Models
{
    public class BrandItem
    {
        [Required]
        public string ItemID { get; set; }
        public string Size { get; internal set; }
        public string ItemDescEng { get; internal set; }
        public List<WarehouseInv> Warehouseinv { get; set; }
        public string Warehousecat { get; internal set; }
    }
}