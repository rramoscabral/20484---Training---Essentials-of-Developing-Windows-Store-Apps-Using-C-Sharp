using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPRestFullClient.Services
{
    public class GetHttp
    {
        /// <summary>
        /// Recebe um enderço url por exemplo http://localhost:13758
        /// 
        /// Código adapatado da documentação do HttpClient https://docs.microsoft.com/en-us/windows/uwp/networking/httpclient
        /// </summary>
        /// <param name="requestUrl endereço URL do tipo string"></param>
        /// <returns>Retorna a resposta do peidido efetuado</returns>
        public async Task<string> TryGetHttpAsync(string requestUrl)
        {
            Uri requestUri = new Uri(requestUrl);
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                //httpResponse = await httpClient.GetAsync(requestUri);

                 httpResponse = httpClient.GetAsync(requestUri).AsTask().Result;

                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                return httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                return httpResponseBody;
            }
        }

        
    }
}
