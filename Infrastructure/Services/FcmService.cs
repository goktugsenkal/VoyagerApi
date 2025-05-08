using System.Collections.ObjectModel;
using Core.Interfaces.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class FcmService : IFcmService
{
    private readonly FirebaseMessaging _messaging;

    public FcmService(IConfiguration configuration)
    {
        // load the path to your service account JSON
        // load the path to your service account JSON
        var credPath = configuration["Firebase:CredentialPath"]
                       ?? throw new InvalidOperationException("Firebase:CredentialPath missing");

        // get the default FirebaseApp if it exists, otherwise create it here
        // this ensures `app` is never null
        var app = FirebaseApp.DefaultInstance
                  ?? FirebaseApp.Create(new AppOptions
                  {
                      Credential = GoogleCredential.FromFile(credPath)
                  });

        // initialize the messaging client from that app
        _messaging = FirebaseMessaging.GetMessaging(app);
    }

    public async Task<string> SendNotificationAsync(string fcmToken,
        string title,
        string body,
        IDictionary<string, string>? data = null)
    {
        // wrap data in a read-only dictionary as you fixed earlier
        var payload = new ReadOnlyDictionary<string, string>(data ?? new Dictionary<string, string>());

        var message = new Message
        {
            Token        = fcmToken,
            Notification = new Notification { Title = title, Body = body },
            Data         = payload,
            FcmOptions = new FcmOptions { AnalyticsLabel = "test-label" }
        };

        return await _messaging.SendAsync(message);
    }
    
    public async Task<string> SendBackgroundNotificationAsync(
        string fcmToken,
        IDictionary<string, string>? data = null)
    {
        // wrap data in a read-only dictionary as you fixed earlier
        var payload = new ReadOnlyDictionary<string, string>(data ?? new Dictionary<string, string>());

        var message = new Message
        {
            Token        = fcmToken,
            Data         = payload,
            FcmOptions = new FcmOptions { AnalyticsLabel = "test-label" }
        };

        return await _messaging.SendAsync(message);
    }
}