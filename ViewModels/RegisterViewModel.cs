using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }

        public string ErrorMessage { get; set; }
        public bool RegisterSuccess { get; set; }
    }
}
