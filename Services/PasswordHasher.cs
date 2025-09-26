using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace OrderSystem.Services
{
    public class PasswordHasher
    {
        /// <summary>
        /// 使用PBKDF2對密碼加密，回傳salt+hash的字串
        /// </summary>
        public static string HashPassword(string password)
        {
            // 產生隨機 Salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // 使用 PBKDF2 雜湊密碼
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // 合併 Salt(放前16) + Hash(放後20)
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // 轉成 Base64 儲存
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// 驗證使用者輸入的密碼是否與資料庫中儲存的雜湊密碼相符
        /// </summary>
        /// <param name="storedHash">資料庫中儲存的密碼雜湊</param>
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // 取出 Salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // 雜湊輸入密碼
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // 比對雜湊值
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}