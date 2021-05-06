using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Launchpad.Models.HomeViewModels
{
    public class LaunchpadViewModel
    {
        public IEnumerable<SelectListItem> Brands { get; set; }
    }
}