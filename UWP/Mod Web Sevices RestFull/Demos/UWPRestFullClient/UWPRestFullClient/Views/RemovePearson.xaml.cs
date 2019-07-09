using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UWPRestFullClient.Data;
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
    public sealed partial class RemovePearson : Page
    {
        private string uriServer = String.Empty;

        public RemovePearson()
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


        private void ListViewPearsons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Click_DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (ListViewPearsons.SelectedItem != null)
            {
                var id = ((UWPRestFullClient.Data.Pearson)ListViewPearsons.SelectedItem).ID.ToString();


                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsondelete/pearson/remove/");
                requestUrl.Append(id);


                StringBuilder jsonDoc = new StringBuilder();
    
                Services.DeleteJsonAsync delete = new Services.DeleteJsonAsync();

                var t = Task.Run(() => delete.TryDeleteJsonAsync(requestUrl.ToString(), jsonDoc.ToString()));
                t.Wait();
                this.tbResult.Text = t.Result;
                InicializeList();
            }
            else
            {
                this.tbResult.Text = "Erro";
            }
        }
    }
}
