using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls.Primitives;

namespace OrderSystem.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            var loginSession = Session["loginSession"];
            var user = Session["user"];
            return View();
        }
    }
}
