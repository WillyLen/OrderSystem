using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class OtpTotpService
    {
        /// <summary>
        /// 產生一組Base32的Secret Key
        /// </summary>
        public string GenerateSecretBase32(int size = 20)
        {
            var secret = KeyGeneration.GenerateRandomKey(size);
            return Base32Encoding.ToString(secret);
        }

        /// <summary>
        /// 用於產生6位數、30秒週期的TOTP碼
        /// </summary>
        public string BuildOtpAuthUri(string issuer, string account, string base32Secret,
                                      int digits = 6, int period = 30)
        {
            // otpauth://totp/Issuer:Account?secret=...&issuer=Issuer&digits=6&period=30
            return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}" +
                   $"?secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}&digits={digits}&period={period}";
        }

        /// <summary>
        /// 驗證使用者輸入的 TOTP 驗證碼是否正確，out var _為忽略驗證成功時的時間區間資訊
        /// </summary>
        /// <param name="base32Secret">使用者的 TOTP 密鑰（Base32 編碼）</param>
        /// <param name="code">使用者輸入的驗證碼（6位數）</param>
        /// <param name="period">驗證碼有效秒數（30秒）</param>
        /// <param name="window">允許的時間誤差範圍（預設 ±1 個時間區間 --> 30秒）</param>
        /// <returns></returns>
        public bool VerifyTotp(string base32Secret, string code, int period = 30, int window = 1)
        {
            var bytes = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(bytes, step: period, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(code, out var _, new VerificationWindow(window, window));
        }
    }
}