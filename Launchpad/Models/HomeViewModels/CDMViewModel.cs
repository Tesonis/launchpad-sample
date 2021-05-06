using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Launchpad.Models;

namespace Launchpad.Models.HomeViewModels
{
    public class CDMViewModel
    {

        public IEnumerable<SelectListItem> Brands { get; set; }

        public string Searchbrand { get; set; }
        public IEnumerable<BrandItem> Items { get; set; }

    }
}