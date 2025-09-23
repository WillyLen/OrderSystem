using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class OtpEnrollment
    {
        public string sessionId { get; set; }
        public string otpCode { get; set; }
        public int expiryTime { get; set; }

    }
}