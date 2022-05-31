using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EdgeOS.API.Core
{
    /// <summary>Provides an API into EdgeOS based off the official API.</summary>
    public class WebClient : IDisposable
    {
        /// <summary>The EdgeOS username used to login.</summary>
        private readonly string username;

        /// <summary>The EdgeOS password used to login.</summary>
        private readonly string password;

        /// <summary>The EdgeOS SessionID returned after logging in.</summary>
        public string SessionID;

        /// <summary>The HTTP Client object that all requests will be performed from. It may have valid credentials pre-configured if <see cref="Login"/> is invoked.</summary>
        private readonly HttpClient _httpClient;
        private readonly CookieContainer cookieContainer = new CookieContainer();
        /// <summary>Creates an instance of the WebClient which can be used to call EdgeOS API methods.</summary>
        /// <param name="username">The username this instance will use to authenticate with the EdgeOS device.</param>
        /// <param name="password">The password this instance will use to authenticate with the EdgeOS device.</param>
        /// <param name="host">The EdgeOS hostname this instance will contact.</param>
        public WebClient(string username, string password, string host)
        {
            // Store the credentials into a private field.
            this.username = username;
            this.password = password;

            // Prevent .NET from consuming the HTTP 303 that contains our authentication session.
            var handler = new HttpClientHandler() { AllowAutoRedirect = false, CookieContainer = cookieContainer };
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };


            // Prevent .NET from consuming the HTTP 303 that contains our authentication session.
            _httpClient = new HttpClient(handler)
            {
                // A EdgeOS API endpoint is the hostname.
                BaseAddress = new Uri(host)
            };

            // Be a good net citizen and reveal who we are.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C#-EdgeOS-API");

            // Allow only our trusted certificate (if there is one) but otherwise reject any certificate errors.
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback.PinPublicKey;
        }

        /// <summary>Attempt to login to the EdgeOS device and configure the <seealso cref="HttpClient"/> with the session credentials for future usage.</summary>
        public void Login()
        {
            // Teardown any previous session.
            SessionID = null;

            // Build up the HTML Form.
            List<KeyValuePair<string, string>> loginForm = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            // Perform the HTTP POST.
            var message = new HttpRequestMessage(HttpMethod.Post, "/");
            message.Content = new FormUrlEncodedContent(loginForm);

            message.Headers.Add("Host", new Uri(_httpClient.BaseAddress.AbsoluteUri).Host);
            message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            message.Headers.Add("Connection", "keep-alive");
            message.Headers.Add("Accept-Encoding", "gzip, deflate, br");

            //message.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            HttpResponseMessage httpResponseMessage = AsyncUtil.RunSync(() => _httpClient.SendAsync(message));


            // The server does not correctly use HTTP Status codes.
            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    string response = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    // Check if the login failed (it likely did).
                    if (response.Contains("The username or password you entered is incorrect")) { throw new FormatException("The username or password you entered is incorrect"); }

                    break;
                case HttpStatusCode.SeeOther:
                    // The response headers will contain the session in a cookie if successful.
                    HttpResponseHeaders headers = httpResponseMessage.Headers;

                    // If for whatever reason login fails then a cookie will not be present.
                    if (!(headers.Contains("Set-Cookie"))) { throw new FormatException("Expected header used for Authentication was not present in the response message back from the server."); }

                    // The stats connection requires the session ID for authentication.
                    const string sessionNeedle = "PHPSESSID=";
                    foreach (string cookie in headers.GetValues("Set-Cookie"))
                    {
                        // We are only interested in the PHPSESSID.
                        if (cookie.StartsWith(sessionNeedle)) {
                            int semicolon = cookie.IndexOf(';');
                            SessionID = semicolon == -1 ? cookie.Substring(sessionNeedle.Length) : cookie.Substring(sessionNeedle.Length, semicolon - sessionNeedle.Length);
                            break;
                        }
                    }

                    // There's a chance the authentication has changed and we are no longer reliant on a PHPSESSID.
                    if (SessionID == null) { throw new FormatException("Unable to find session credentials."); }

                    break;
            }
        }


        public void ClearTraffic()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/edge/operation/clear-traffic-analysis.json");
           
            message.Headers.Add("Host", new Uri(_httpClient.BaseAddress.AbsoluteUri).Host);
            message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            message.Headers.Add("Connection", "keep-alive");
            message.Headers.Add("Accept-Encoding", "gzip, deflate, br");            
            cookieContainer.Add(new Uri("https://" + new Uri(_httpClient.BaseAddress.AbsoluteUri).Host),new Cookie( "PHPSESSID" , this.SessionID));
            message.Headers.Add("X-Requested-With", "XMLHttpRequest");
            HttpResponseMessage httpResponseMessage = AsyncUtil.RunSync(() => _httpClient.SendAsync(message));
            
        }

        /// <summary>Ensures proper clean up of the resources.</summary>
        public void Dispose()
        {
            if (_httpClient != null) { _httpClient.Dispose(); }
        }
    }
}