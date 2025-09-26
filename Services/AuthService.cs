using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class AuthService
    {
        /// <summary>
        /// 帳號密碼驗證
        /// </summary>
        public bool LoginAuthCheck(string account, string password)
        {
            var dao = new UserDao();
            var user = dao.GetUserByAccount(account);

            if (user == null)
                return false;

            return PasswordHasher.VerifyPassword(password, user.password);
        }

        /// <summary>
        /// 註冊資訊檢查
        /// </summary>
        public string RegisterCheck(string email, string account, string password)
        {
            var dao = new UserDao();
            var isDuplicateAccount = dao.VerifyDuplicateAccount(account);
            var isDuplicateEmail = dao.VerifyDuplicateEmail(email);
            if (!isDuplicateEmail)
            {
                return !isDuplicateAccount ? "Success": "AccountAlreadyExists";
            }
            else
            {
                return "EmailAlreadyExists";
            }
        }
    }
}