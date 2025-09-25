using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class OtpTotpService
    {
        public string GenerateSecretBase32(int size = 20)
        {
            var secret = KeyGeneration.GenerateRandomKey(size);
            return Base32Encoding.ToString(secret);
        }

        public string BuildOtpAuthUri(string issuer, string account, string base32Secret,
                                      int digits = 6, int period = 30)
        {
            // otpauth://totp/Issuer:Account?secret=...&issuer=Issuer&digits=6&period=30
            return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}" +
                   $"?secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}&digits={digits}&period={period}";
        }

        public bool Verify(string base32Secret, string code, int period = 30, int window = 1)
        {
            var bytes = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(bytes, step: period, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(code, out var _, new VerificationWindow(window, window));
        }
    }
}