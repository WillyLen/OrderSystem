using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models
{
    public class LoginSession
    {
        public string sessionId { get; set; }
        public string account { get; set; }
        public DateTime expiry { get; set; }
        public bool isUsed { get; set; }
        public DateTime createdAt { get; set; }
    }
}