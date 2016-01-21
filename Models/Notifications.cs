using Microsoft.Azure.NotificationHubs;
using System;

public class Notifications
{
    public static Notifications Instance = new Notifications();

    public NotificationHubClient Hub { get; set; }

    private Notifications()
    {
        Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://homesecurityhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=/zIgiUyNvlgnIxFInyvQUNFLeIYZcjWK/IAnmx7Ryoo=",
                                                                     "homesecurityhub");
    }
}