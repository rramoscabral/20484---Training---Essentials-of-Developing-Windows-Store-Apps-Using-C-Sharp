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
    public class PutJsonAsync
    {
        public async Task<string> TryPutJsonAsync(string requestUrl, string jsonDoc)
        {
          try
            {
                // Construct the HttpClient and Uri. This endpoint is for test purposes only.
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri(requestUrl);

                // Construct the JSON to Put.
                HttpStringContent content = new HttpStringContent(
                    jsonDoc,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json");

                // Put the JSON and wait for a response.
                // HttpResponseMessage httpResponseMessage  = httpClient.PutAsync(uri, content).GetResults();
                HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(uri, content);

                // Make sure the Put succeeded, and write out the response.
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
