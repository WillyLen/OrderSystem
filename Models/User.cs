using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class User
    {
        public string account { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; } // Admin / User
        public DateTime createdAt { get; set; }
        public string TotpSecret { get; set; }   // Base32
        public bool TwoFactorEnabled { get; set; }

    }
}