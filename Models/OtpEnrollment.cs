using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class OtpEnrollment
    {
        public string sessionId { get; set; } // 對應登入動作的sessionId
        public string otpCode   { get; set; } // OTP碼
        public int expiryTime   { get; set; } // OTP碼有效時間(default 60秒)
    }
}