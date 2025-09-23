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
            if (string.IsNullOrEmpty(account) && string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}