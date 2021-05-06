using System;
using System.ComponentModel.DataAnnotations;

namespace Launchpad.Models
{
    public class WarehouseInv : BrandItem
    {
        [Required]
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int CurrentInventory { get; set; }
        public DateTime NextPODate { get; set; }
    }
}