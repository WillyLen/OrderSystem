using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class AuthService
    {
        public bool LoginAuthCheck(string account, string password)
        {
            var dao = new UserDao();
            var user = dao.GetUserByAccount(account);

            if (user == null)
                return false;

            return PasswordHasher.VerifyPassword(password, user.password);
        }

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