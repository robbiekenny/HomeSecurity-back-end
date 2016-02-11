using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Security.Claims;
using homesecurityService.DataObjects;
using homesecurityService.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace homesecurityService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class CustomLoginController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler handler { get; set; }
        ResponseMessage rm = new ResponseMessage();

        // POST api/CustomLogin
        public HttpResponseMessage Post(LoginRequest loginRequest)
        {
            homesecurityContext context = new homesecurityContext();
            Account account = context.Accounts
                .Where(a => a.Email == loginRequest.Email).SingleOrDefault();
            if (account != null)
            {
                byte[] incoming = CustomLoginProviderUtils
                    .hash(loginRequest.Password, account.Salt);

                if (CustomLoginProviderUtils.slowEquals(incoming, account.SaltedAndHashedPassword))
                {
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginRequest.Email));
                    LoginResult loginResult = new CustomLoginProvider(handler)
                        .CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                    var customLoginResult = new CustomLoginResult()
                    {
                        UserId = loginResult.User.UserId,
                        MobileServiceAuthenticationToken = loginResult.AuthenticationToken,
                        Verified = account.Verified
                    };
                    return this.Request.CreateResponse(HttpStatusCode.OK, customLoginResult);
                }
            }
            var message = "Fail";
            return this.Request.CreateResponse(HttpStatusCode.Unauthorized,
                new { message });
        }
    }
}
