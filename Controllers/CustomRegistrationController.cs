using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Text.RegularExpressions;
using homesecurityService.DataObjects;
using homesecurityService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Net;
using System.Net.Mail;
using RestSharp;
using RestSharp.Authenticators;
//using SendGrid;

/*REGISTER A USER BY SAVING THEIR EMAIL ADDRESS AND PASSWORD PROVIDED THE EMAIL DOESNT ALREADY EXIST
A USERS PASSWORD IS SAVED BY GENERATING A SALT AND HASHING THE PASSWORD USING THAT SALT
THE SALT IS SAVED ALONG WITH THE HASHED PASSWORD

UPON SUCCESSFUL REGISTRATION A USER WILL BE SENT AN EMAIL ASKING THEM TO VERIFY THEIR ACCOUNT 
SO THAT THEY CAN USE THE HOMESECURITY APPLICATION
*/

namespace homesecurityService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class CustomRegistrationController : ApiController
    {
        public ApiServices ApiServices { get; set; }
        ResponseMessage rm = new ResponseMessage();

        // POST api/CustomRegistration
        public HttpResponseMessage Post(RegistrationRequest registrationRequest)
        {
            var message = "Fail";
            homesecurityContext context = new homesecurityContext();
            if(registrationRequest != null)
            {
                Account account = context.Accounts.Where(a => a.Email == registrationRequest.email).SingleOrDefault();
                if (account != null)
                {
                    //account already exists
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
                }
                else
                {
                    //create new account
                    byte[] salt = CustomLoginProviderUtils.generateSalt();
                    Account newAccount = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = registrationRequest.email,
                        Salt = salt,
                        SaltedAndHashedPassword = CustomLoginProviderUtils.hash(registrationRequest.password, salt),
                        Verified = false
                    };
                    context.Accounts.Add(newAccount);
                    context.SaveChanges();
                    rm.message = "Created";
                    rm.verified = false;
                    try
                    {
                        //send email to this email address so that a user may verify it
                        VerifyAccountEmail(registrationRequest.email);
                    }catch(Exception e)
                    {
                        ApiServices.Log.Error("ERROR SENDING EMAIL");
                    }
                   
                    return this.Request.CreateResponse(HttpStatusCode.Created, rm);
                }
            }
            message = "Failed";
            return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
        }

        public static IRestResponse VerifyAccountEmail(string email)
        {
            //encode the streaming url so that its not visible in the url
            var encodedUrl = System.Text.Encoding.UTF8.GetBytes(email);

            string homeSecurtityWebsite = "homesecurityapp.azurewebsites.net/VerifyAccount/Index?email=" + System.Convert.ToBase64String(encodedUrl);
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-7849f01c422d48638073c526a5b69ee1");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "sandbox09debf1633754ea39b8f70e8c5dc3a35.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Home Security <homesecurity@security.ie>");
            request.AddParameter("to", email);
            
            request.AddParameter("subject", "Verify Account");
            
            request.AddParameter("html", "<html>Hi there,<br>Verifying your HomeSecurity will allow us to email you images of potential break ins etc."+
            "To verify your account simply follow this " +
            "<a href=" + homeSecurtityWebsite + ">link</a> </html>");
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }   
}
