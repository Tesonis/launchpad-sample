using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IBM.Data.DB2.iSeries;
using Launchpad.Models.AccountViewModels;
using TOLC.ERP.Application;

namespace Launchpad.Controllers
{
    public class AccountController : Controller
    {
        public object Formathentication { get; private set; }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Login(string usrUserID, string usrPassword, string signon)
        {
            if (usrUserID != null && usrPassword != null)
            {
                ReturnValue rv = new ReturnValue();
                Session session = null;
                rv = new Security().Logon(usrUserID, usrPassword, ref session,false);
                if (session == null)
                {
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    HttpCookie userCookie = new HttpCookie("SecToken");
                    userCookie["FullName"] = session.FullName;
                    userCookie["Email"] = session.EmailAddress;
                    userCookie["Username"] = session.Username;
                    userCookie["SecurityKey"] = session.securityIdentifier;
                    userCookie["Role"] = session.Team.ToString();
                    userCookie.Expires.AddHours(1);
                    Response.SetCookie(userCookie);
                    FormsAuthentication.SetAuthCookie(userCookie["SecurityKey"], false);
                    return RedirectToAction("Launchpad", "Home");
                }
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult LoginAJAX(string username, string password, string role)
        {
                ReturnValue rv = new ReturnValue();
                Session session = null;
                rv = new Security().Logon(username, password, ref session,false);
                if (session == null)
                {
                    return Json(new {status = "error",errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    HttpCookie userCookie = new HttpCookie("SecToken");
                    userCookie["FullName"] = session.FullName;
                    userCookie["Email"] = session.EmailAddress;
                    userCookie["Username"] = session.Username;
                    userCookie["SecurityKey"] = session.securityIdentifier;
                    userCookie["Role"] = session.Team.ToString();
                    userCookie.Expires.AddHours(1);
                    Response.SetCookie(userCookie);
                    FormsAuthentication.SetAuthCookie(userCookie["SecurityKey"], false);
                    return Json(new { url = Url.Action("Launchpad", "Home") }, JsonRequestBehavior.AllowGet);
                }
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["SecToken"] != null)
            {
                Response.Cookies["SecToken"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}