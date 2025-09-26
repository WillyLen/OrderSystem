using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class LoginSession
    {
        public string sessionId   { get; set; } // 對應登入動作的sessionId
        public string account     { get; set; } // 帳號
        public DateTime expiry    { get; set; } // session有效時間(DateTime)
        public bool isUsed        { get; set; } // session是否已使用(true或false)
        public DateTime createdAt { get; set; } // 建立時間(DateTime)
    }
}