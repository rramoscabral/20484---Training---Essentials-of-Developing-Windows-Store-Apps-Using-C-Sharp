using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace UWPRestFullClient.Services
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/uwp/networking/httpclient
    /// </summary>
    /// <returns></returns>
    /// 
    public class PostJsonAsync
    {
        public async Task<string> TryPostJsonAsync(string requestUrl, string jsonDoc)
        {
          try
            {
                // Construct the HttpClient and Uri. This endpoint is for test purposes only.
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri(requestUrl);

                // Construct the JSON to post.
                HttpStringContent content = new HttpStringContent(
                    jsonDoc,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json");

                // Post the JSON and wait for a response.
                // HttpResponseMessage httpResponseMessage  = httpClient.PostAsync(uri, content).GetResults();
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, content);

                // Make sure the post succeeded, and write out the response.
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                return httpResponseBody;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
