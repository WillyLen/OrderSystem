using OrderSystem.Models;
using OrderSystem.Models.ViewModels;
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
        private bool IsSessionValid(LoginSession session, string sessionId)
        {
            return session != null &&
                   !session.isUsed &&
                   DateTime.Now <= session.expiry &&
                   session.sessionId == sessionId;
        }

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
        public ActionResult Login(string sessionId)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(sessionId);

            if (!IsSessionValid(session, sessionId))
            {
                string script = "<script>alert('QRCode 已過期或無效'); window.location.href = '/Auth/index';</script>";
                return Content(script, "text/html");
            }

            var model = new LoginViewModel
            {
                SessionId = session.sessionId
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(model.SessionId);

            if (!IsSessionValid(session, model.SessionId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var authService = new OrderSystem.Services.AuthService();
            bool isValid = authService.LoginAuthCheck(model.Account, model.Password);

            if (isValid)
            {
                dao.MarkAsUsedBySessionId(session.sessionId);
                dao.MarkAccount(session.sessionId, model.Account);
                return RedirectToAction("ShowOtp", new { sessionId = model.SessionId });
            }
            else
            {
                model.ErrorMessage = "帳號或密碼錯誤!";
                return View(model);
            }
        }

        //public ActionResult Register(string sessionId)
        //{
        //    var dao = new LoginSessionDao();
        //    var session = dao.GetBySessionId(sessionId);

        //    if (!IsSessionValid(session, sessionId))
        //    {
        //        return RedirectToAction("Index", "Auth");
        //    }

        //    ViewBag.SessionId = sessionId;
        //    return View();
        //}
        public ActionResult Register(string sessionId)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(sessionId);

            if (!IsSessionValid(session, sessionId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var model = new RegisterViewModel
            {
                SessionId = sessionId
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            var dao = new UserDao();
            var authService = new OrderSystem.Services.AuthService();
            string result = authService.RegisterCheck(model.Email, model.Account, model.Password);

            if (result == "Success")
            {
                string hashedPassword = PasswordHasher.HashPassword(model.Password);

                User user = new User
                {
                    email = model.Email,
                    account = model.Account,
                    password = hashedPassword,
                    role = "User",
                    createdAt = DateTime.Now
                };

                dao.AddUser(user);
                model.RegisterSuccess = true;
                return View(model);
            }
            else if (result == "EmailAlreadyExists")
            {
                ViewBag.ErrorMessage = "Email already exists.";
            }
            else
            {
                ViewBag.ErrorMessage = "Account already exists.";
            }
            return View(model);
        }

        public ActionResult ShowOtp(string sessionId)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(sessionId);

            if (session.sessionId != sessionId)
            {
                return RedirectToAction("index", "Auth");
            }

            var otpService = new OtpService();
            var otpModel = otpService.GenerateOtp(sessionId); // 產生並儲存 OTP 到 DB

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(otpModel);
        }

        [HttpPost]
        public ActionResult VerifyOtp(string sessionId, string inputOtp)
        {
            var otpService = new OtpService();
            bool isValid = otpService.ValidateOtp(sessionId, inputOtp);

            if (isValid)
            {
                otpService.DeleteOtp(sessionId); // 驗證成功後刪除 OTP
                return RedirectToAction("Index", "Menu");
            }
            else
            {
                TempData["ErrorMessage"] = "驗證失敗，已產生新的 OTP。";
                return RedirectToAction("ShowOtp", new { sessionId = sessionId });
            }
        }
    }
}