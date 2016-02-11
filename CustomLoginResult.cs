using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace homesecurityService
{
    public class CustomLoginResult
    {
        public string UserId { get; set; }
        public string MobileServiceAuthenticationToken { get; set; }
        public bool Verified { get; set; } //added so that i can check if a users account is verified
    }
}