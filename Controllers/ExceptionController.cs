using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderSystem.Controllers
{
    public class ExceptionController : Controller
    {
        public ActionResult Logout()
        {
            return View();
        }
        public ActionResult SessionTimeOut()
        {
            return View();
        }
    }
}