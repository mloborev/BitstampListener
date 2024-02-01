using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Modules.GoogleIntegrator
{
    public static class GoogleAuth
    {
        public static async Task<UserCredential> LoginAsync(string googleClientId,  string googleClientSecret, string[] scopes)
        {
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, "maxim.mloborev@gmail.com", CancellationToken.None);
        }
    }
}
