using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using homesecurityService.Models;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AppBackend.Controllers
{
    public class NotificationsController : ApiController
    {
        public ApiServices ApiServices { get; set; }
        
        public async Task<HttpResponseMessage> Post(string pns, [FromBody]string message, string to_tag)
        {
            //var user = HttpContext.Current.User.Identity.Name;
            ApiServices.Log.Error("to_tag: " + to_tag + "\t Time: " + DateTime.Now);
            ApiServices.Log.Info("message: " + message + "\t Time: " + DateTime.Now);
            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;
            userTag[1] = "from:" + to_tag;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;
            
            switch (pns.ToLower())
            {
               // case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                //    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                //                "From " + user + ": " + message + "</text></binding></visual></toast>";
                //    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                //    break;
                //case "apns":
                //    // iOS
                //    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                //    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                //    break;
                case "gcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                    break;
            }
            ApiServices.Log.Info("In notifications controller, before the if statement\t Time: " + DateTime.Now);
            if (outcome != null)
            {
                ApiServices.Log.Info("In notifications controller, in the if statement\t Time: " + DateTime.Now);
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ApiServices.Log.Info("In notifications controller, in the inner if statement\t Time: " + DateTime.Now);
                    ret = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse(ret);
        }
    }
}
