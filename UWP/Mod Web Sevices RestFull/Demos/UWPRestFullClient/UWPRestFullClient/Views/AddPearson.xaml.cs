using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UWPRestFullClient.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPRestFullClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPearson : Page
    {

        private string uriServer = String.Empty;

        public AddPearson()
        {
            this.InitializeComponent();
        }

        private void Button_Click_CreateRecord(object sender, RoutedEventArgs e)
        {

            try
            {
                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonregistration/InsertPearson");

                StringBuilder jsonDoc = new StringBuilder();
                jsonDoc.Append("{");
                jsonDoc.Append($"   \"ID\":\"{tBID.Text}\",");
                jsonDoc.Append($"   \"Name\":\"{tBName.Text}\",");
                jsonDoc.Append($"   \"Email\":\"{tBEmail.Text}\"");
                jsonDoc.Append("}");


                Services.PostJsonAsync postJSon = new Services.PostJsonAsync();

                var t = Task.Run(() => postJSon.TryPostJsonAsync(requestUrl.ToString(), jsonDoc.ToString()));
                t.Wait();
                this.tbResult.Text = t.Result;
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.Message;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            uriServer = e.Parameter as string;
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Click_CreateRecordNewtonsoft(object sender, RoutedEventArgs e)
        {
            try
            {

                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonregistration/InsertPearson");

                Uri uri = new Uri(requestUrl.ToString());


               // var payload = "{ \"ID\":\"{tBID.Text}\", \"Name\":\"{tBName.Text}\",  \"Email\":\"{tBEmail.Text}\" }";


                var payload = new Dictionary<string, string>
                {
                    {"ID", tBID.Text},
                    {"Name", tBName.Text},
                    {"Email", tBEmail.Text}

                };

                // Requer o Newtonsoft.Json
                string strPayload = JsonConvert.SerializeObject(payload);

                HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PostJsonNewtonsoftAsync.PostURI(uri, content));
                t.Wait();

                this.tbResult.Text = t.Result;
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.Message;
            }
        }
    }
}

