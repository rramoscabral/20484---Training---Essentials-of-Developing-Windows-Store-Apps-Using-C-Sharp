using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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
    public sealed partial class GetAllPearsons : Page
    {
        private string uriServer = String.Empty;

        public GetAllPearsons()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder requestUrl = new StringBuilder();
                requestUrl.Append(uriServer);
                requestUrl.Append("/api/pearsonretrive");


                Services.GetHttp get = new Services.GetHttp();


                Task<string> httpGetRequest = get.TryGetHttpAsync(requestUrl.ToString());
                string results = httpGetRequest.Result;

                //string httpGetRequest = get.TryGetHttpAsync(requestUrl.ToString()).Result;

                //JSON.NET Nuget: Newtonsoft.Json
                // JsonConvert.DeserializeObject<Pearson>(results);



                textblockResutld.Text = results;
            }catch (Exception ex)
            {
                textblockResutld.Text = ex.Message;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            uriServer = e.Parameter as string;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
