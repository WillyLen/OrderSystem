using OrderSystem.Models;
using OrderSystem.Models.ViewModels;
using OrderSystem.Repositories;
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
        /// <summary>
        /// 用於確認是否存在有效的session以及是否為同一個sessionId，若為false則導回Auth/index
        /// </summary>
        private bool IsSessionValid(LoginSession session, string sessionId)
            {
            return session != null &&
                   !session.isUsed &&
                   DateTime.Now <= session.expiry &&
                   session.sessionId == sessionId;
        }

        /// <summary>
        /// 顯示往登入頁面的QR Code
        /// </summary>
        public ActionResult Index()
        {

            var qrService = new OrderSystem.Services.QrService();
            var (qrCodeImagedata, qrText, loginSession) = qrService.GenerateQRCode();

            ViewBag.QrImage = qrCodeImagedata;
            ViewBag.QrText = qrText;
            Session["loginSession"] = loginSession;

            return View();
        }

        /// <summary>
        /// 用於更新Auth/index的QR Code
        /// </summary>
        public ActionResult GetNewQrCode()
        {
            var qrService = new OrderSystem.Services.QrService();
            var (qrCodeImagedata, qrText, loginSession) = qrService.GenerateQRCode();

            ViewBag.QrImage = qrCodeImagedata;
            ViewBag.QrText = qrText;
            Session["loginSession"] = loginSession;

            return PartialView("QrCodePartial");
        }

        /// <summary>
        /// 顯示登入頁表格
        /// </summary>
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

        /// <summary>
        /// 為Login頁表格的POST動作
        /// </summary>
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(model.SessionId);

            if (!IsSessionValid(session, model.SessionId))
                return RedirectToAction("Index", "Auth");

            var authService = new OrderSystem.Services.AuthService();
            bool isValid = authService.LoginAuthCheck(model.Account, model.Password);

            if (!isValid)
            {
                model.ErrorMessage = "帳號或密碼錯誤!";
                return View(model);
            }

            // 標記 session 已綁帳號
            dao.MarkAccount(session.sessionId, model.Account);

            // 改導向到QrcodeAuth
            return RedirectToAction("QrcodeAuth", new { sessionId = model.SessionId });
        }

        /// <summary>
        /// 此頁面為登入後的OTP驗證方法一，若使用者尚未啟用TOTP則顯示QR Code供掃描，若已啟用則顯示輸入6碼的表格(Using Authenticator App)
        /// </summary>
        public ActionResult QrcodeAuth(string sessionId)
        {
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            if (!IsSessionValid(sess, sessionId)) return RedirectToAction("Index");

            var userDao = new UserDao();
            var user = userDao.GetUserByAccount(sess.account);

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

        /// <summary>
        /// 啟用Authenticator功能(以後就不需要掃QR Code，直接輸入OTP)
        /// </summary>
        /// <param name="code">User輸入的OTP Code</param>
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
            if (!totpSvc.VerifyTotp(base32, code))
            {
                TempData["MfaError"] = "驗證碼錯誤，請再試一次。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            //把secret寫進使用者，TwoFactorEnabled = 1
            userDao.UpdateTotp(user.account, base32, true);
            TempData["MfaInfo"] = "已成功啟用 Authenticator。";
            return RedirectToAction("QrcodeAuth", new { sessionId });
        }

        /// <summary>
        /// 檢查User輸入的OTP是否正確，正確則登入成功，並儲存JWT及Refresh Token到Cookie
        /// </summary>
        /// <param name="code">User輸入的OTP Code</param>
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
            if (!totpSvc.VerifyTotp(user.TotpSecret, code))
            {
                TempData["MfaError"] = "TOTP 驗證失敗。";
                return RedirectToAction("QrcodeAuth", new { sessionId });
            }

            // 登入成功後產生 JWT
            string token = JwtHelper.GenerateToken(user.account, user.role);

            // 儲存 JWT 到 Cookie（HttpOnly + SameSite）
            var cookie = new HttpCookie("jwt", token)
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(30)
            };
            Response.Cookies.Add(cookie);

            // 登入成功後產生 Refresh Token
            string refreshToken = Guid.NewGuid().ToString();
            DateTime refreshExpiry = DateTime.Now.AddDays(7);
            new RefreshTokenDao().SaveToken(user.account, refreshToken, refreshExpiry);

            // 儲存到 Cookie
            var refreshCookie = new HttpCookie("refresh_token", refreshToken)
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshExpiry
            };

            // 標記 session 已使用過
            sessDao.MarkAsUsedBySessionId(sess.sessionId);

            return RedirectToAction("Index", "Menu");
        }

        /// <summary>
        /// 註冊帳號頁面
        /// </summary>
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

        /// <summary>
        /// 檢查帳號及信箱是否重複，無誤則新增使用者
        /// </summary>
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

        /// <summary>
        /// 此頁面為登入後的OTP驗證方法二，使用者能使用切換按鈕換至此頁面，系統會產生一組OTP並顯示給使用者，使用者輸入後驗證。
        /// </summary>
        public ActionResult ShowOtp(string sessionId)
        {
            var dao = new LoginSessionDao();
            var session = dao.GetBySessionId(sessionId);

            if (session.sessionId != sessionId)
            {
                return RedirectToAction("index", "Auth");
            }

            var otpService = new OtpService();
            otpService.DeleteOtp(sessionId);
            var otpModel = otpService.GenerateOtp(sessionId); // 產生並儲存 OTP 到 DB

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(otpModel);
        }

        /// <summary>
        /// 驗證使用者輸入的OTP是否正確，失敗則重整頁面並產生新的OTP，成功登入後儲存JWT及Refresh Token到Cookie
        /// </summary>
        [HttpPost]
        public ActionResult VerifyOtp(string sessionId, string inputOtp)
        {
            var sessDao = new LoginSessionDao();
            var sess = sessDao.GetBySessionId(sessionId);
            var otpService = new OtpService();
            bool isValid = otpService.ValidateOtp(sessionId, inputOtp);

            if (isValid)
            {
                var userDao = new UserDao();
                var user = userDao.GetUserByAccount(sess.account);


                // 登入成功後產生 JWT
                string token = JwtHelper.GenerateToken(user.account, user.role);

                // 儲存 JWT 到 Cookie（HttpOnly + SameSite）
                var cookie = new HttpCookie("jwt", token)
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Add(cookie);

                // 登入成功後產生 Refresh Token
                string refreshToken = Guid.NewGuid().ToString();
                DateTime refreshExpiry = DateTime.Now.AddDays(7);
                new RefreshTokenDao().SaveToken(user.account, refreshToken, refreshExpiry);

                // 儲存到 Cookie
                var refreshCookie = new HttpCookie("refresh_token", refreshToken)
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshExpiry
                };
                Response.Cookies.Add(refreshCookie);
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

