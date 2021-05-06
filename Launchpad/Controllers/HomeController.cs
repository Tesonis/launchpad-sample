using IBM.Data.DB2.iSeries;
using Launchpad.Models;
using Launchpad.Models.HomeViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TOLC.ERP.Application;
using KeyAccount = Launchpad.Models.KeyAccount;


namespace Launchpad.Controllers
{

    public class HomeController : Controller
    {
        //public ActionResult Test()
        //{
        //    return View();
        //}

        /// <summary>
        /// Index View
        /// </summary>
        /// <param name="view">view based on user role</param>
        /// <returns>Role specific page layout</returns>
        public ActionResult Launchpad(string view)
        {
            LaunchpadViewModel vm = new LaunchpadViewModel
            {
                Brands = GetBrands()
            };
            ViewBag.name = Request.Cookies["SecToken"]["FullName"];
            return View(vm);
        }

        /// <summary>
        /// LaunchBDM Page Controller
        /// </summary>
        /// <param name="a">create or update action for different variant layout</param>
        /// <returns></returns>
        public ActionResult LaunchBDM(string a)
        {
            BDMViewModel vm = new BDMViewModel
            {
                Brands = GetBrands()
            };
            ViewBag.a = a.ToString();
            return View(vm);
        }

        /// <summary>
        /// LaunchCDM View Controller
        /// </summary>
        /// <param name="a">create or update action for different variant layout</param>
        /// <returns></returns>
        public ActionResult LaunchCDM(string a)
        {
            CDMViewModel vm = new CDMViewModel
            {
                Brands = GetBrands()
            };
            ViewBag.a = a.ToString();
            return View(vm);
        }

        /// <summary>
        /// Retrieves unlaunched items in the under the Launchpad items table
        /// </summary>
        /// <param name="brand">brand code</param>
        /// <returns>List of items to be put on to Datatable</returns>
        [HttpGet]
        public ActionResult GetBrandItems(string brand)
        {       
                
                List<BrandItem> itemlist = new List<BrandItem>();
                DataSet DBSetBranditems = null;
                ReturnValue rv = new ReturnValue();
                TempData["errorcode"] = "0";

                    //Calls API Method with brand parameter
                    rv = new Item().ListbyBrand(Request.Cookies["SecToken"]["SecurityKey"], brand, ref DBSetBranditems);
                    if (rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                    }

                    if (DBSetBranditems != null)
                    {
                        foreach (DataTable table in DBSetBranditems.Tables)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                BrandItem item = new BrandItem
                                {
                                    //refer to rowid in akitm minus 1
                                    ItemID = row["ITEM"].ToString(),
                                    ItemDescEng = row["ITEM_DESCRIPTION"].ToString(),
                                    Size = row["ITEM_SIZE"].ToString()
                                };
                                itemlist.Add(item);
                            }
                        }
                    }
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Item Record View Controller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ItemRecord()
        {
            LaunchpadViewModel vm = new LaunchpadViewModel();
            ViewBag.test = "Get Successful";
            return PartialView("_ItemRecord",vm);
        }
        /// <summary>
        /// Item Report View Controller - Unused in Production
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ItemReport(string id)
        {
            ViewBag.currentdate = DateTime.Now.ToShortDateString();
            return View();
        }

        /// <summary>
        /// Item Report Partial View Controller - Not Used in Production
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ItemReportPartial(string id)
        {
            return View();
        }

        /// <summary>
        /// Retrieve list of accessible brands based on user
        /// Brand Managers have access to their brands, while Key account managers have access to all brands
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetBrands()
        {
            List<SelectListItem> brandlist = new List<SelectListItem>();
            Brand Brands = new Brand();
            DataSet DBSetBrands = null;
            //Brands.List(HttpContext.Session["SecurityKey"].ToString(), ref DBSetBrands);
            Brands.ListbyBrandManager(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetBrands);

            if (DBSetBrands != null)
            {
                foreach (DataTable table in DBSetBrands.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var brand = new SelectListItem
                        {
                            Value = row["PRNPRN"].ToString(),
                            Text = row["PRNNAM"].ToString()
                        };
                        brandlist.Add(brand);
                    }
                }
            }
            else
            {
                TempData["errorcode"] = 911;
            }
            Brands = null;
            return brandlist;
        }
        public ActionResult ListBrandsbyBDM()
        {
            List<SelectListItem> brandlist = new List<SelectListItem>();
            Brand Brands = new Brand();
            DataSet DBSetBrands = null;
            //Brands.List(HttpContext.Session["SecurityKey"].ToString(), ref DBSetBrands);
            Brands.ListbyBrandManager(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetBrands);

            if (DBSetBrands != null)
            {
                foreach (DataTable table in DBSetBrands.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var brand = new SelectListItem
                        {
                            Value = row["PRNPRN"].ToString(),
                            Text = row["PRNNAM"].ToString()
                        };
                        brandlist.Add(brand);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(brandlist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }


        //AJAX FORM ACTIONS
        /// <summary>
        /// Retrieve inventory of list of items grouped by warehouse locations
        /// </summary>
        /// <param name="arr">List of items in a string array</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RetrieveInv(string arr)
        {
            List<string> itemids = JsonConvert.DeserializeObject<List<string>>(arr);
            List<BrandItem> iteminv = new List<BrandItem>();
            DataSet DBSetItemInv = new DataSet();

            ReturnValue rv = new ReturnValue();
            rv = new Item().ListInventory(Request.Cookies["SecToken"]["SecurityKey"], itemids, ref DBSetItemInv);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetItemInv != null)
            {
                foreach (DataTable table in DBSetItemInv.Tables)
                {
                    var temp = new BrandItem();
                    foreach (DataRow row in table.Rows)
                    {
                        int.TryParse(row[3].ToString(), out int warehousecode);
                        
                        if (row[3].ToString() == "1")
                        {
                            if (temp.ItemID != null)
                            {
                                iteminv.Add(temp);
                            }
                            
                            temp = new BrandItem
                            {
                                ItemID = (string)row[0],
                                Warehouseinv = new List<WarehouseInv>()
                            };
                        }
                        var tempwarehouseinv = new WarehouseInv
                        {
                            WarehouseID = warehousecode,
                            WarehouseName = row[4].ToString()
                        };
                        if (float.TryParse(row[5].ToString(), out float inventory))
                        {
                            tempwarehouseinv.CurrentInventory = (int)inventory;
                        }
                        else
                        {
                            tempwarehouseinv.CurrentInventory = 0;
                        }
                        if (row[6].ToString() == "")
                        {
                            tempwarehouseinv.NextPODate = DateTime.MinValue;
                        }
                        else
                        {
                            DateTime.TryParse(row[6].ToString(), out DateTime nextpo);
                            tempwarehouseinv.NextPODate = nextpo;
                        }
                        temp.Warehouseinv.Add(tempwarehouseinv);
                    }
                    if (temp.ItemID != null)
                    {
                        iteminv.Add(temp);
                    }
                }
                
            }

            string returnstring = JsonConvert.SerializeObject(iteminv);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        /// Listing Items from brands that have not been launched through launchpad program
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public ActionResult ListUnlaunchedItems(string brand)
        {
            //use brand string to query list of items
            List<BrandItem> itemlist = new List<BrandItem>();
            DataSet DBSetBranditems = null;
            ReturnValue rv = new ReturnValue();
            TempData["errorcode"] = "0";
            //rv = new ItemLaunch().ListUnlaunched(Request.Cookies["SecToken"]["SecurityKey"], brand, ref DBSetBranditems);
            rv = new ItemLaunch().ListUnlaunched(Request.Cookies["SecToken"]["SecurityKey"], brand, ref DBSetBranditems);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetBranditems != null)
            {
                foreach (DataTable table in DBSetBranditems.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        BrandItem item = new BrandItem
                        {
                            //refer to rowid in akitm minus 1
                            ItemID = row["ITMITM"].ToString(),
                            ItemDescEng = row["ITMDSE"].ToString(),
                            //ItemDescFR Might be used
                            Size = row["ITMDSS"].ToString(),
                            Warehousecat = row["ITMUBC"].ToString()
                        };
                        itemlist.Add(item);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(itemlist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListLaunchTypes()
        {
            List<SelectListItem> launchTypeList = new List<SelectListItem>();
            DataSet DBSetLaunchTypes = null;
            ReturnValue rv = new ReturnValue();
            //call method
            rv = new ItemLaunchType().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetLaunchTypes);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetLaunchTypes != null)
            {
                foreach (DataTable table in DBSetLaunchTypes.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var launchtype = new SelectListItem
                        {
                            Value = row["ROWID"].ToString(),
                            Text = row["TEXT"].ToString()
                        };
                        launchTypeList.Add(launchtype);
                    }
                }
            }
            
            string returnstring = JsonConvert.SerializeObject(launchTypeList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListSampleAvail()
        {
            List<SelectListItem> sampleAvailList = new List<SelectListItem>();
            DataSet DBSetSampleAvail = null;
            ReturnValue rv = new ReturnValue();
            //call method
            rv = new ItemSampleAvailability().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetSampleAvail);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetSampleAvail != null)
            {
                foreach (DataTable table in DBSetSampleAvail.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var sampleavail = new SelectListItem
                        {
                            Value = row["ROWID"].ToString(),
                            Text = row["TEXT"].ToString()

                        };
                        sampleAvailList.Add(sampleavail);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(sampleAvailList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListComments(string launchid)
        {
            List<LaunchComment> listComments = new List<LaunchComment>();
            DataSet DBSetComments = null;
            ReturnValue rv = new ReturnValue();
            rv = new ItemLaunch().ListComments(Request.Cookies["SecToken"]["SecurityKey"], int.Parse(launchid), ref DBSetComments);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetComments != null)
            {
                foreach (DataTable table in DBSetComments.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        LaunchComment c = new LaunchComment
                        {
                            CommentText = row["COMMENT"].ToString(),
                            CommentUser = row["CREATE_USERID"].ToString(),
                            CommentDate = DateTime.Parse(row["CREATE_TS"].ToString())
                        };
                        listComments.Add(c);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(listComments);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetKeyAccounts()
        {
            List<KeyAccount> KeyAccountList = new List<KeyAccount>();
            //TOLC.ERP.Account account = new Account();
            ReturnValue rv = new ReturnValue();
            DataSet DBSetKeyAccounts = null;
            rv = new TOLC.ERP.Application.KeyAccount().List(Request.Cookies["SecToken"]["SecurityKey"],0, ref DBSetKeyAccounts);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetKeyAccounts != null)
            {
                foreach (DataTable table in DBSetKeyAccounts.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        KeyAccount k = new KeyAccount
                        {
                            AccountCode = row["KACKAC"].ToString(),
                            AccountName = row["KACNAM"].ToString()
                        };
                        KeyAccountList.Add(k);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(KeyAccountList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListLaunchStatus()
        {
            List<SelectListItem> LaunchStatusList = new List<SelectListItem>();
            DataSet DBSetLaunchStatus = null;
            ReturnValue rv = new ReturnValue();
            //call method
            rv = new ItemLaunchStatus().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetLaunchStatus);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetLaunchStatus != null)
            {
                foreach (DataTable table in DBSetLaunchStatus.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var launchstatus = new SelectListItem
                        {
                            Value = row["ROWID"].ToString(),
                            Text = row["TEXT"].ToString()
                        };
                        LaunchStatusList.Add(launchstatus);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(LaunchStatusList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submitlaunch(string launchtype, string items, string customeracc)
        {
            List<ItemLaunchPayload> payload = new List<ItemLaunchPayload>();
            var serializer = new JavaScriptSerializer();
            dynamic itemdynamic = JsonConvert.DeserializeObject(items);
            dynamic kacdynamic = JsonConvert.DeserializeObject(customeracc);
            
            foreach (var i in itemdynamic)
            {
                List<KeyAccountLaunchPayload> kaclaunchpayload = new List<KeyAccountLaunchPayload>();
                foreach (var j in kacdynamic)
                {
                    KeyAccountLaunchPayload kacpayload = new KeyAccountLaunchPayload
                    {
                        KeyAccount = j.Customercode.Value,
                        comment = i.Comment.Value,
                    };
                    if (int.TryParse(j.Status.Value, out int initstatus))
                    {
                        switch (initstatus)
                        {
                            case 1:
                                kacpayload.status = ItemLaunch.Status.NA;
                                break;
                            case 2:
                                kacpayload.status = ItemLaunch.Status.DiscussionRequired;
                                break;
                            default:
                                kacpayload.status = ItemLaunch.Status.ReadyToPresent;
                                break;
                        }
                    }
                    kaclaunchpayload.Add(kacpayload);
                }
                ItemLaunchPayload itemlaunchpayload = new ItemLaunchPayload
                {
                    item = i.Itemid.Value,
                    keyAccounts =kaclaunchpayload
                };
                if (int.TryParse(i.Sampleavail.Value, out int sampleavail))
                {
                    switch (sampleavail)
                    {
                        case 1:
                            itemlaunchpayload.sampleAvailability = ItemLaunch.SampleAvailability.FullDistribution;
                            break;
                        case 2:
                            itemlaunchpayload.sampleAvailability = ItemLaunch.SampleAvailability.LimitedDistribution;
                            break;
                        case 3:
                            itemlaunchpayload.sampleAvailability = ItemLaunch.SampleAvailability.PackagingFlatsMockUps;
                            break;
                        case 4:
                            itemlaunchpayload.sampleAvailability = ItemLaunch.SampleAvailability.AvailableUponRequest;
                            break;
                        default:
                            itemlaunchpayload.sampleAvailability = ItemLaunch.SampleAvailability.NoSamples;
                            break;
                    }
                }
                if (int.TryParse(launchtype, out int launchtypecode))
                {
                    switch (launchtypecode)
                    {
                        case 1:
                            itemlaunchpayload.type = ItemLaunch.Type.LineExtension;
                            break;
                        case 2:
                            itemlaunchpayload.type = ItemLaunch.Type.NewBrand;
                            break;
                        default:
                            itemlaunchpayload.type = ItemLaunch.Type.PreLaunch;
                            break;
                    }
                }
                payload.Add(itemlaunchpayload);
            }
            ReturnValue rv = new ReturnValue();
            rv = new ItemLaunch().Create(Request.Cookies["SecToken"]["SecurityKey"], ref payload);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { url = Url.Action("Launchpad", "Home") });
            }
        }
        [HttpPost]
        public ActionResult SubmitUpdateBDM(string updatejson)
        {
            var serializer = new JavaScriptSerializer();
            dynamic launchdynamic = JsonConvert.DeserializeObject(updatejson);
            ReturnValue rv = new ReturnValue();
            foreach (var i in launchdynamic)
            {
                string launchid = i.Launchid.Value;
                string comment = i.Comment.Value;
                ItemLaunch.SampleAvailability sampleavailability = new ItemLaunch.SampleAvailability();
                ItemLaunch.Status updatestatus = new ItemLaunch.Status();
                ItemLaunch.Type updatelaunchtype = new ItemLaunch.Type();
                if (int.TryParse(i.Sampleavail.Value, out int sampleavail))
                {
                    switch (sampleavail)
                    {
                        case 1:
                            sampleavailability = ItemLaunch.SampleAvailability.FullDistribution;
                            break;
                        case 2:
                            sampleavailability = ItemLaunch.SampleAvailability.LimitedDistribution;
                            break;
                        case 3:
                            sampleavailability = ItemLaunch.SampleAvailability.PackagingFlatsMockUps;
                            break;
                        case 4:
                            sampleavailability = ItemLaunch.SampleAvailability.AvailableUponRequest;
                            break;
                        default:
                            sampleavailability = ItemLaunch.SampleAvailability.NoSamples;
                            break;
                    }
                }
                if (int.TryParse(i.Launchtype.Value, out int launchtypecode))
                {
                    switch (launchtypecode)
                    {
                        case 1:
                            updatelaunchtype = ItemLaunch.Type.LineExtension;
                            break;
                        case 2:
                            updatelaunchtype = ItemLaunch.Type.NewBrand;
                            break;
                        default:
                            updatelaunchtype = ItemLaunch.Type.PreLaunch;
                            break;
                    }
                }
                if (int.TryParse(i.Status.Value, out int initstatus))
                {
                    switch (initstatus)
                    {
                        case 1:
                            updatestatus = ItemLaunch.Status.NA;
                            break;
                        case 2:
                            updatestatus = ItemLaunch.Status.DiscussionRequired;
                            break;
                        default:
                            updatestatus = ItemLaunch.Status.ReadyToPresent;
                            break;
                    }
                }
                rv = new ItemLaunch().Update(Request.Cookies["SecToken"]["SecurityKey"], int.Parse(launchid), updatelaunchtype, sampleavailability, updatestatus, comment);
                if (rv.Number != 0)
                {
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { url = Url.Action("Launchpad", "Home") });
        }
        [HttpPost]
        public ActionResult SubmitUpdateCDM(string updatejson)
        {
            var serializer = new JavaScriptSerializer();
            dynamic launchdynamic = JsonConvert.DeserializeObject(updatejson);
            ReturnValue rv = new ReturnValue();
            foreach (var i in launchdynamic)
            {
                string launchid = i.Launchid.Value;
                string category = i.Category.Value;
                DateTime.TryParse(i.Pogreview.Value, out DateTime pogreview);
                DateTime.TryParse(i.Pogdrop.Value, out DateTime pogdrop);
                string comment = i.Comment.Value;
                string categorycomment = i.Categorycomment.Value;
                string competitive = i.Competitive.Value;
                string declinedreason = i.Declinedreason.Value;
                DateTime.TryParse(i.Followup.Value, out DateTime followup);
                ItemLaunch.Status updatestatus = new ItemLaunch.Status();
                
                if (int.TryParse(i.Status.Value, out int initstatus))
                {
                    switch (initstatus)
                    {
                        case 3:
                            updatestatus = ItemLaunch.Status.ReadyToPresent;
                            break;
                        case 4:
                            updatestatus = ItemLaunch.Status.VORChangeInitiated;
                            break;
                        case 5:
                            updatestatus = ItemLaunch.Status.VORChangeCompleted;
                            break;
                        case 7:
                            updatestatus = ItemLaunch.Status.InitialContact;
                            break;
                        case 8:
                            updatestatus = ItemLaunch.Status.InitialMeeting;
                            break;
                        case 9:
                            updatestatus = ItemLaunch.Status.LaunchPackageSent;
                            break;
                        case 10:
                            updatestatus = ItemLaunch.Status.Pending;
                            break;
                        case 11:
                            updatestatus = ItemLaunch.Status.Accepted;
                            break;
                        case 12:
                            updatestatus = ItemLaunch.Status.DeclinedByRetailer;
                            break;
                        default:
                            updatestatus = ItemLaunch.Status.DeclinedByClient;
                            break;
                    }
                }
                rv = new ItemLaunch().Update(Request.Cookies["SecToken"]["SecurityKey"],int.Parse(launchid),updatestatus,category,pogreview,pogdrop,categorycomment,competitive,followup,int.Parse(declinedreason),comment);
                if (rv.Number != 0)
                {
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { url = Url.Action("Launchpad", "Home") });
        }
        public ActionResult ListBDMTeam(string user)
        {
            List<SelectListItem> BDMTeamList = new List<SelectListItem>();
            DataSet DBSetBDMTeam = null;
            ReturnValue rv = new ReturnValue();
            //call method
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetBDMTeam != null)
            {
                foreach (DataTable table in DBSetBDMTeam.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var teammember = new SelectListItem
                        {
                            //Value = launchvalue,
                        };
                        BDMTeamList.Add(teammember);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(BDMTeamList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListBDMLaunches(string src, string brand, string kac, string banner, string statusjson)
        {
            List<ItemLaunch.Status> statuslist = new List<ItemLaunch.Status>();
            if (statusjson != "[]" && statusjson != null)
            {
                List<string> statusids = JsonConvert.DeserializeObject<List<string>>(statusjson);
                foreach (var i in statusids)
                {
                    switch (i)
                    {
                        case "1":
                            statuslist.Add(ItemLaunch.Status.NA);
                            break;
                        case "2":
                            statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                            break;
                        case "3":
                            statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                            break;
                        case "4":
                            statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                            break;
                        case "5":
                            statuslist.Add(ItemLaunch.Status.VORChangeCompleted);
                            break;
                        case "7":
                            statuslist.Add(ItemLaunch.Status.InitialContact);
                            break;
                        case "8":
                            statuslist.Add(ItemLaunch.Status.InitialMeeting);
                            break;
                        case "9":
                            statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                            break;
                        case "10":
                            statuslist.Add(ItemLaunch.Status.Pending);
                            break;
                        case "11":
                            statuslist.Add(ItemLaunch.Status.Accepted);
                            break;
                        case "12":
                            statuslist.Add(ItemLaunch.Status.DeclinedByRetailer);
                            break;
                        case "13":
                            statuslist.Add(ItemLaunch.Status.DeclinedByClient);
                            break;
                        default:

                            break;

                    }
                }
            }
            else
            {
                if (src == "inprogress")
                {
                    statuslist.Add(ItemLaunch.Status.InitialContact);
                    statuslist.Add(ItemLaunch.Status.InitialMeeting);
                    statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                    statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                    statuslist.Add(ItemLaunch.Status.Pending);
                }
                else if (src == "task")
                {
                    statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                }
                else if (src == "bdmupdate")
                {
                    statuslist.Add(ItemLaunch.Status.NA);
                    statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                    statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                }
                else
                {
                    statuslist.Add(ItemLaunch.Status.Accepted);
                    statuslist.Add(ItemLaunch.Status.DeclinedByClient);
                    statuslist.Add(ItemLaunch.Status.DeclinedByRetailer);
                    statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                    statuslist.Add(ItemLaunch.Status.InitialContact);
                    statuslist.Add(ItemLaunch.Status.InitialMeeting);
                    statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                    statuslist.Add(ItemLaunch.Status.NA);
                    statuslist.Add(ItemLaunch.Status.Pending);
                    statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                    statuslist.Add(ItemLaunch.Status.VORChangeCompleted);
                    statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                }
            }
            
            List<LaunchRecord> BDMLaunchList = new List<LaunchRecord>();
            DataSet DBSetBDMLaunches = null;
            ReturnValue rv = new ReturnValue();
            if (brand == "") { brand = null; }
            if (kac == "") { kac = null; }
            if (banner == "") { banner = null; }
            rv = new ItemLaunch().ListBrandManagerLaunches(Request.Cookies["SecToken"]["SecurityKey"], statuslist, brand, kac, banner, ref DBSetBDMLaunches);
            //call method
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetBDMLaunches != null)
            {
                foreach (DataTable table in DBSetBDMLaunches.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var launch = new LaunchRecord
                        {
                            LaunchID = row["LAUNCH_ID"].ToString(),
                            Itemnumber = row["ITEM"].ToString(),
                            Itemdesc = row["ITEM_DESCRIPTION"].ToString(),
                            Itemsize = row["ITEM_SIZE"].ToString(),
                            CaseUPC = row["CASE_UPC"].ToString(),
                            Brandcode = row["BRAND"].ToString(),
                            Brandname = row["BRAND_NAME"].ToString(),
                            BDM = row["BDM"].ToString(),
                            Customeracccode = row["CUSTOMER_ACCOUNT"].ToString(),
                            Customeraccname = row["CUSTOMER_ACCOUNT_NAME"].ToString(),
                            CDM = row["CDM"].ToString(),
                            Bannercode = row["BANNER"].ToString(),
                            Bannername = row["BANNER_NAME"].ToString(),
                            Launchtype = row["LAUNCH_TYPE_TEXT"].ToString(),
                            Sampleavail = row["SAMPLE_AVAILABILITY_TEXT"].ToString(),
                            Status = row["STATUS_TEXT"].ToString(),
                            Launchdate = row["LAUNCH_TS"].ToString(),
                            Launchuser = row["LAUNCH_USERID"].ToString(),
                            Productcategory = row["PRODUCT_CATEGORY"].ToString(),
                            Pogreview = row["PLANOGRAM_REVIEW_TS"].ToString(),
                            Pogdrop = row["PLANOGRAM_DROP_TS"].ToString(),
                            Categorycomment = row["CONTACT_COMMENT"].ToString(),
                            Competitive = row["COMPETITIVE_INFO"].ToString(),
                            Followup = row["FOLLOW_UP_TS"].ToString()
                            //Map all values to model
                        };
                        BDMLaunchList.Add(launch);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(BDMLaunchList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListCDMLaunches(string src, string brand, string kac, string banner, string statusjson)
        {
            List<ItemLaunch.Status> statuslist = new List<ItemLaunch.Status>();
            if (statusjson != "[]" && statusjson != null)
            {
                List<string> statusids = JsonConvert.DeserializeObject<List<string>>(statusjson);
                foreach (var i in statusids)
                {
                    switch (i)
                    {
                        case "1":
                            statuslist.Add(ItemLaunch.Status.NA);
                            break;
                        case "2":
                            statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                            break;
                        case "3":
                            statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                            break;
                        case "4":
                            statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                            break;
                        case "5":
                            statuslist.Add(ItemLaunch.Status.VORChangeCompleted);
                            break;
                        case "7":
                            statuslist.Add(ItemLaunch.Status.InitialContact);
                            break;
                        case "8":
                            statuslist.Add(ItemLaunch.Status.InitialMeeting);
                            break;
                        case "9":
                            statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                            break;
                        case "10":
                            statuslist.Add(ItemLaunch.Status.Pending);
                            break;
                        case "11":
                            statuslist.Add(ItemLaunch.Status.Accepted);
                            break;
                        case "12":
                            statuslist.Add(ItemLaunch.Status.DeclinedByRetailer);
                            break;
                        case "13":
                            statuslist.Add(ItemLaunch.Status.DeclinedByClient);
                            break;
                        default:
                            break;

                    }
                }
            }
            else
            {
                if (src == "inprogress")
                {
                    statuslist.Add(ItemLaunch.Status.InitialContact);
                    statuslist.Add(ItemLaunch.Status.InitialMeeting);
                    statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                    statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                    statuslist.Add(ItemLaunch.Status.Pending);
                    //Remove This Status
                    statuslist.Add(ItemLaunch.Status.Accepted);
                }
                else if (src == "task")
                {
                    statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                }
                else if (src == "cdminit")
                {
                    statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                }
                else
                {
                    statuslist.Add(ItemLaunch.Status.Accepted);
                    statuslist.Add(ItemLaunch.Status.DeclinedByClient);
                    statuslist.Add(ItemLaunch.Status.DeclinedByRetailer);
                    statuslist.Add(ItemLaunch.Status.DiscussionRequired);
                    statuslist.Add(ItemLaunch.Status.InitialContact);
                    statuslist.Add(ItemLaunch.Status.InitialMeeting);
                    statuslist.Add(ItemLaunch.Status.LaunchPackageSent);
                    statuslist.Add(ItemLaunch.Status.NA);
                    statuslist.Add(ItemLaunch.Status.Pending);
                    statuslist.Add(ItemLaunch.Status.ReadyToPresent);
                    statuslist.Add(ItemLaunch.Status.VORChangeCompleted);
                    statuslist.Add(ItemLaunch.Status.VORChangeInitiated);
                }
            }
            
            List<LaunchRecord> CDMLaunchList = new List<LaunchRecord>();
            DataSet DBSetCDMLaunches = null;
            ReturnValue rv = new ReturnValue();

            rv = new ItemLaunch().ListKeyAccountManagerLaunches(Request.Cookies["SecToken"]["SecurityKey"], statuslist, brand, kac, banner, ref DBSetCDMLaunches);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetCDMLaunches != null)
            {
                foreach (DataTable table in DBSetCDMLaunches.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var launch = new LaunchRecord
                        {
                            LaunchID = row["LAUNCH_ID"].ToString(),
                            Itemnumber = row["ITEM"].ToString(),
                            Itemdesc = row["ITEM_DESCRIPTION"].ToString(),
                            Itemsize = row["ITEM_SIZE"].ToString(),
                            CaseUPC = row["CASE_UPC"].ToString(),
                            Brandcode = row["BRAND"].ToString(),
                            Brandname = row["BRAND_NAME"].ToString(),
                            BDM = row["BDM"].ToString(),
                            Customeracccode = row["CUSTOMER_ACCOUNT"].ToString(),
                            Customeraccname = row["CUSTOMER_ACCOUNT_NAME"].ToString(),
                            CDM = row["CDM"].ToString(),
                            Bannercode = row["BANNER"].ToString(),
                            Bannername = row["BANNER_NAME"].ToString(),
                            Launchtype = row["LAUNCH_TYPE_TEXT"].ToString(),
                            Sampleavail = row["SAMPLE_AVAILABILITY_TEXT"].ToString(),
                            Status = row["STATUS_TEXT"].ToString(),
                            Launchdate = row["LAUNCH_TS"].ToString(),
                            Launchuser = row["LAUNCH_USERID"].ToString(),
                            Productcategory = row["PRODUCT_CATEGORY"].ToString(),
                            Pogreview = row["PLANOGRAM_REVIEW_TS"].ToString(),
                            Pogdrop = row["PLANOGRAM_DROP_TS"].ToString(),
                            Categorycomment = row["CONTACT_COMMENT"].ToString(),
                            Competitive = row["COMPETITIVE_INFO"].ToString(),
                            Followup = row["FOLLOW_UP_TS"].ToString()
                            //Map all values to model
                        };
                        CDMLaunchList.Add(launch);
                    }
                }
            }
            //TEMP UNTIL KACLAUNCHES WORK


            string returnstring = JsonConvert.SerializeObject(CDMLaunchList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBannerFromAcc(string kac)
        {
            List<SelectListItem> BannerList = new List<SelectListItem>();
            DataSet DBSetBanner = null;
            ReturnValue rv = new ReturnValue();
            //call method
            rv = new Banner().List(Request.Cookies["SecToken"]["SecurityKey"], kac, ref DBSetBanner);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            if (DBSetBanner != null)
            {
                foreach (DataTable table in DBSetBanner.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var banner = new SelectListItem
                        {
                            Value = row["KSSKSS"].ToString(),
                            Text = row["KSSKSN"].ToString()
                        };
                        BannerList.Add(banner);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(BannerList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListStatusTimeline(string id)
        {
            List<Statustimeline> StatusTSList = new List<Statustimeline>();
            DataSet DBSetStatusTS = null;
            ReturnValue rv = new ReturnValue();
            if (int.TryParse(id, out int launchid)){
                rv = new ItemLaunch().ListStatus(Request.Cookies["SecToken"]["SecurityKey"], launchid, ref DBSetStatusTS);
                if (rv.Number != 0)
                {
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
                if (DBSetStatusTS != null)
                {
                    foreach (DataTable table in DBSetStatusTS.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            var statusts = new Statustimeline
                            {
                                Statuscode = row["SORT_ID"].ToString(),
                                Status = row["TEXT"].ToString(),
                                TSstring = row["UPDATE_TS"].ToString()
                            };
                            StatusTSList.Add(statusts);
                        }
                    }
                }
            }
            
            
            string returnstring = JsonConvert.SerializeObject(StatusTSList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListDeclinedReason()
        {
            List<SelectListItem> DeclinedReasonList  = new List<SelectListItem>();
            DataSet DBSetReason = null;
            ReturnValue rv = new ReturnValue();
                rv = new ItemLaunchStatus().ListReason(Request.Cookies["SecToken"]["SecurityKey"], 12, ref DBSetReason);
                if (rv.Number != 0)
                {
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
                if (DBSetReason != null)
                {
                    foreach (DataTable table in DBSetReason.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            var declinedreason = new SelectListItem
                            {
                                Value = (float.Parse(row["SORT_ID"].ToString())+1).ToString(),
                                Text = row["TEXT"].ToString()
                            };
                            DeclinedReasonList.Add(declinedreason);
                        }
                    }
                }


            string returnstring = JsonConvert.SerializeObject(DeclinedReasonList);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
    }
}