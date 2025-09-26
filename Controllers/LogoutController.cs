using OrderSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderSystem.Controllers
{
    public class LogoutController : Controller
    {
        /// <summary>
        /// 清除 Session 及 Cookie，並刪除資料庫中的 Refresh Token後導向登出成功頁面
        /// </summary>
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            // 清除 Refresh Token（資料庫）
            var refreshToken = Request.Cookies["refresh_token"]?.Value;
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var dao = new RefreshTokenDao();
                var account = dao.GetAccountByToken(refreshToken);
                if (account != null)
                {
                    dao.DeleteToken(account); // 刪除資料庫中的 Refresh Token
                }
            }

            // 清除 Cookie
            Response.Cookies.Add(new HttpCookie("jwt") { Expires = DateTime.Now.AddDays(-1) });
            Response.Cookies.Add(new HttpCookie("refresh_token") { Expires = DateTime.Now.AddDays(-1) });

            return View();
        }

        /// <summary>
        /// 逾時強制登出頁面
        /// </summary>
        public ActionResult SessionTimeOut()
        {
            return View();
        }
    }
}