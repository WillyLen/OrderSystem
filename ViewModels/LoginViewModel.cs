using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.ViewModels
{
    public class LoginViewModel
    {
        public string account { get; set; }
        public string role { get; set; }
        public string sessionId { get; set; }
    }
}