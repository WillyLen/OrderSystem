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


        // [POST] /Auth/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(model.SessionId);

            if (!IsSessionValid(session, model.SessionId))
                return RedirectToAction("Index", "Auth");

            var authService = new OrderSystem.Services.AuthService();
            bool isValid = authService.LoginAuthCheck(model.Account, model.Password); // :contentReference[oaicite:10]{index=10}

            if (!isValid)
            {
                model.ErrorMessage = "帳號或密碼錯誤!";
                return View(model);
            }

            // 標記 session 已綁帳號
            dao.MarkAccount(session.sessionId, model.Account);

            // NEW: 改導向到QrcodeAuth
            return RedirectToAction("QrcodeAuth", new { sessionId = model.SessionId });
        }
        // [GET] /Auth/ChooseMfa?sessionId=...
        public ActionResult QrcodeAuth(string sessionId)
        {
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            if (!IsSessionValid(sess, sessionId)) return RedirectToAction("Index");

            var userDao = new UserDao();
            var user = userDao.GetUserByAccount(sess.account); // 讀使用者:contentReference[oaicite:1]{index=1}

            ViewBag.SessionId = sessionId;
            ViewBag.TotpEnabled = user?.TwoFactorEnabled == true;

            if (user != null && !user.TwoFactorEnabled)
            {
                var totpSvc = new OtpTotpService();
                var base32 = totpSvc.GenerateSecretBase32();
                var uri = totpSvc.BuildOtpAuthUri("OrderSystem", user.account, base32);
                TempData["__EnrollSecret"] = base32;
                ViewBag.ManualKey = base32;

                using (var qrGen = new QRCodeGenerator())
                using (var qrData = qrGen.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q))
                using (var pngQr = new PngByteQRCode(qrData))
                {
                    byte[] pngBytes = pngQr.GetGraphic(5);
                    string b64 = Convert.ToBase64String(pngBytes);
                    ViewBag.QrDataUrl = "data:image/png;base64," + b64;
                }
            }
            return View();
        }

        // [POST] /Auth/EnableTotp  （用於首次掃描後輸入一組 6 碼確認開通）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnableTotp(string sessionId, string code)
        {
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            if (!IsSessionValid(sess, sessionId)) return RedirectToAction("Index");

            var userDao = new UserDao();
            var user = userDao.GetUserByAccount(sess.account);

            var base32 = TempData["__EnrollSecret"] as string;
            if (string.IsNullOrEmpty(base32))
            {
                TempData["MfaError"] = "啟用流程已逾時，請重新產生 QR。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            var totpSvc = new OtpTotpService();
            if (!totpSvc.Verify(base32, code))
            {
                TempData["MfaError"] = "驗證碼錯誤，請再試一次。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            // 開通信號：把 secret 寫進使用者，TwoFactorEnabled = 1
            userDao.UpdateTotp(user.account, base32, true);
            TempData["MfaInfo"] = "已成功啟用 Authenticator。";
            return RedirectToAction("QrcodeAuth", new { sessionId });
        }

        // [POST] /Auth/VerifyTotp  （已啟用者每次登入時輸入 6 碼）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyTotp(string sessionId, string code)
        {
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            if (!IsSessionValid(sess, sessionId)) return RedirectToAction("Index");

            var userDao = new UserDao();
            var user = userDao.GetUserByAccount(sess.account);

            if (!(user?.TwoFactorEnabled ?? false) || string.IsNullOrEmpty(user.TotpSecret))
            {
                TempData["MfaError"] = "尚未啟用 Authenticator。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            var totpSvc = new OtpTotpService();
            if (!totpSvc.Verify(user.TotpSecret, code))
            {
                TempData["MfaError"] = "TOTP 驗證失敗。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            // 標記 session 已使用過
            sessDao.MarkAsUsedBySessionId(sess.sessionId);

            // 通過：登入完成
            return RedirectToAction("Index", "Menu");
        }

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
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            var otpService = new OtpService();
            bool isValid = otpService.ValidateOtp(sessionId, inputOtp);

            if (isValid)
            {
                sessDao.MarkAsUsedBySessionId(sess.sessionId); // 標記 session 已使用過
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