using OrderSystem.Models;
using OrderSystem.Services;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderSystem.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {

            var qrService = new OrderSystem.Services.QrService();
            var (qrCodeImagedata, qrText, loginSession) = qrService.GenerateQRCode();

            ViewBag.QrImage = qrCodeImagedata;
            ViewBag.QrText = qrText;
            Session["loginSession"] = loginSession;

            return View();
        }
        public ActionResult GetNewQrCode()
        {
            var qrService = new OrderSystem.Services.QrService();
            var (qrCodeImagedata, qrText, loginSession) = qrService.GenerateQRCode();

            ViewBag.QrImage = qrCodeImagedata;
            ViewBag.QrText = qrText;
            Session["loginSession"] = loginSession;

            return PartialView("QrCodePartial");
        }
        public ActionResult LoginAndRegister(string sessionId)
        {
            var loginSession = Session["loginSession"] as Models.LoginSession;
            if(loginSession.sessionId != sessionId)
            {
                return RedirectToAction("index", "Auth");
            }

            if (loginSession == null || loginSession.isUsed || DateTime.Now > loginSession.expiry)
            {
                return Content("QRCode 已過期或無效");
            }

            ViewBag.sessionId = loginSession.sessionId;
            return View();
        }

        [HttpPost]
        public ActionResult LoginAndRegister(string account, string password,  string sessionId)
        {
            var loginSession = Session["loginSession"] as Models.LoginSession;
            if (loginSession.sessionId != sessionId)
            {
                return RedirectToAction("index", "Auth");
            }
            loginSession.account = account;

            User user = new User
            {
                account = account,
                password = password
            };
            Session["user"] = user;

            var authService = new OrderSystem.Services.AuthService();
            bool isValid = authService.LoginAuthCheck(account, password);
            if (isValid)
            {
                loginSession.isUsed = true;
                Session["loginSession"] = loginSession;
                return RedirectToAction("ShowOtp", new { sessionId = sessionId });
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid account or password.";
                ViewBag.SessionId = sessionId;
                Session["loginSession"] = loginSession;
                return View();
            }
        }
        public ActionResult ShowOtp(string sessionId)
        {
            var loginSession = Session["loginSession"] as Models.LoginSession;
            if (loginSession.sessionId != sessionId)
            {
                return RedirectToAction("index", "Auth");
            }

            var otpService = new OtpService();
            var otpModel = otpService.GenerateOtp(sessionId); // 產生 OTP

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(otpModel); // 傳給 View 顯示
        }

        [HttpPost]
        public ActionResult VerifyOtp(string sessionId, string inputOtp)
        {
            var otpService = new OtpService();
            bool isValid = otpService.ValidateOtp(sessionId, inputOtp);

            if (isValid)
            {
                return RedirectToAction("index", "Menu");
            }
            else
            {
                TempData["ErrorMessage"] = "驗證失敗，已重新產生新的 OTP。";
                return RedirectToAction("ShowOtp", new { sessionId = sessionId });
            }
        }
    }
}