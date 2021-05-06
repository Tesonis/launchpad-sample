using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Launchpad.Models
{
    public class LaunchRecord
    {

        public string Itemnumber { get; set; }
        public string Itemdesc { get; set; }
        public string Itemsize { get; set; }
        public string Brandcode { get; set; }
        public string Brandname { get; set; }
        public string Customeracccode { get; set; }
        public string Customeraccname { get; set; }
        public string Bannercode { get; set; }
        public string Bannername { get; set; }
        public string Status { get; set; }
        public string Launchdate { get; set; }
        public string Launchuser { get; set; }
        public string BDM { get; internal set; }
        public string CDM { get; internal set; }
        public string Launchtype { get; internal set; }
        public string Sampleavail { get; internal set; }
        public string LaunchID { get; internal set; }
        public string Pogreview { get; internal set; }
        public string Pogdrop { get; internal set; }
        public string Productcategory { get; internal set; }
        public string Categorycomment { get; internal set; }
        public string Competitive { get; internal set; }
        public string Followup { get; internal set; }
        public string CaseUPC { get; internal set; }
    }
}