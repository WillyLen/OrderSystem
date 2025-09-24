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
            return true;
        }
        public bool RegisterCheck(string email, string account, string password)
        {
            return true;
        }
    }
}