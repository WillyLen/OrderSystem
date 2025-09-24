using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderSystem.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return View();
        }
        public ActionResult SessionTimeOut()
        {
            return View();
        }
    }
}