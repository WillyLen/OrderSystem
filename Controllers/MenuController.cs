using OrderSystem.Models.ViewModels;
using OrderSystem.Repositories;
using OrderSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls.Primitives;

namespace OrderSystem.Controllers
{
    public class MenuController : Controller
    {
        public ActionResult Index()
        {
            var token = Request.Cookies["jwt"]?.Value;

            if (string.IsNullOrEmpty(token))
            {
                // 嘗試使用 Refresh Token 續期
                var refreshToken = Request.Cookies["refresh_token"]?.Value;
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var dao = new RefreshTokenDao();
                    var account = dao.GetAccountByToken(refreshToken); // 你需要實作這個方法
                    if (account != null)
                    {
                        var user = new UserDao().GetUserByAccount(account);
                        string newAccessToken = JwtHelper.GenerateToken(user.account, user.role);
                        Response.Cookies.Add(new HttpCookie("jwt", newAccessToken));
                        token = newAccessToken; // 更新 token 變數
                    }
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var principal = JwtHelper.ValidateToken(token);
                var account = principal.Identity.Name;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                ViewBag.Account = account;
                ViewBag.Role = role;

                return View();
            }
            catch
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }

}
