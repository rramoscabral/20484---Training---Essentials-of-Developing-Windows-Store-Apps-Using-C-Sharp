using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UWPRestFullClient.Data;
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
    public sealed partial class UpdatePearson : Page
    {
        private string uriServer = String.Empty;


        public UpdatePearson()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            uriServer = e.Parameter as string;
            InicializeList();
        }


        private string GetAllData()
        {
            try
            {
                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonretrive");
                Services.GetHttp get = new Services.GetHttp();
                Task<string> httpGetRequest = get.TryGetHttpAsync(requestUrl.ToString());
                return httpGetRequest.Result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private JObject GetAllDataJson()
        {
            JObject json = JObject.Parse(GetAllData());
            return json;
        }


        private void InicializeList()
        {
            try
            {
                List<Pearson> listPearsons = JsonConvert.DeserializeObject<List<Pearson>>(GetAllData());
                this.ListViewPearsons.ItemsSource = listPearsons;
            }
            catch (Exception)
            {
                this.tbResult.Text = "Sem ligação ao Web Service.";
            }
        }


        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void ListViewPearsons_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ListViewPearsons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPearsons.SelectedItem != null)
            {
                this.tBID.Text = ((UWPRestFullClient.Data.Pearson)ListViewPearsons.SelectedItem).ID.ToString();
                this.tBName.Text = ((UWPRestFullClient.Data.Pearson)ListViewPearsons.SelectedItem).Name;
                this.tBEmail.Text = ((UWPRestFullClient.Data.Pearson)ListViewPearsons.SelectedItem).Email;
            }
            else
            {
                this.tBID.Text = String.Empty;
                this.tBName.Text = String.Empty;
                this.tBEmail.Text = String.Empty;
            }
        }

        private void Button_Click_UpdatePearsonNewtonsoft(object sender, RoutedEventArgs e)
        {
            try
            {

                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonupdate");

                Uri uri = new Uri(requestUrl.ToString());

                var payload = new Dictionary<string, string>
                {
                    {"ID", tBID.Text},
                    {"Name", tBName.Text},
                    {"Email", tBEmail.Text}

                };

                // Requer o Newtonsoft.Json
                string strPayload = JsonConvert.SerializeObject(payload);

                HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PutJsonNewtonsoftAsync.PutURI(uri, content));
                t.Wait();

                this.tbResult.Text = t.Result;
                InicializeList();
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.Message;
            }
        }

        private void Button_Click_UpdatePearson(object sender, RoutedEventArgs e)
        {
            try
            {

                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonupdate");

                StringBuilder jsonDoc = new StringBuilder();
                jsonDoc.Append("{");
                jsonDoc.Append($"   \"ID\":\"{tBID.Text}\",");
                jsonDoc.Append($"   \"Name\":\"{tBName.Text}\",");
                jsonDoc.Append($"   \"Email\":\"{tBEmail.Text}\"");
                jsonDoc.Append("}");


                Services.PutJsonAsync postJSon = new Services.PutJsonAsync();

                var t = Task.Run(() => postJSon.TryPutJsonAsync(requestUrl.ToString(), jsonDoc.ToString()));
                t.Wait();

                this.tbResult.Text = t.Result;
                InicializeList();
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.Message;
            }
        }
    }
}

