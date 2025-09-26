using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class User
    {
        public string account        { get; set; } // 帳號
        public string password       { get; set; } // 密碼(加密)
        public string email          { get; set; } // 信箱
        public string role           { get; set; } // 角色權限(Admin / User)
        public DateTime createdAt    { get; set; } // 建立時間
        public string TotpSecret     { get; set; } // 建立Totp的密鑰
        public bool TwoFactorEnabled { get; set; } // 雙因子驗證啟用與否(1或0)
    }

}