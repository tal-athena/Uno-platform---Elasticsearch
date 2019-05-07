using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using Elasticsearch.Net;
using System.Net.Http;
using System.Net;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        private ObservableCollection<ServiceInfo> lstServiceInfos;
        private ObservableCollection<Abschluse> m_lstPdfInfos;

        public MainPage()
        {
            InitializeComponent();

            lstServiceInfos = new ObservableCollection<ServiceInfo>();
            lstService.ItemsSource = lstServiceInfos;


            m_lstPdfInfos = new ObservableCollection<Abschluse>();
            myDataGrid.ItemsSource = m_lstPdfInfos;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            ServiceInfo srvInfo = (ServiceInfo)lstService.SelectedItem;
            
            if (srvInfo == null)
                return;
                
            m_lstPdfInfos.Clear();
            OutputPane.Text = "";
            Console.WriteLine("SearchButton: "+srvInfo.URL + "/" + srvInfo.Index + "/" + srvInfo.Type + "/_search=" + txtSearch.Text);
            searchResult(srvInfo.URL, srvInfo.Index, srvInfo.Type, txtSearch.Text);
            //searchResult("http://localhost:9200/", "gba", "abschluse", txtSearch.Text);

        }

        private async void searchResult(string srv_url, string index, string type, string srch)
        {
            if (srv_url[srv_url.Length - 1] != '/')
                srv_url += "/";

            var settings = new ConnectionConfiguration(new Uri(srv_url + index + "/" + type + "/"))
                                .RequestTimeout(TimeSpan.FromMinutes(2))                                
                                .ConnectionLimit(-1);

            var lowlevelClient = new ElasticLowLevelClient(settings);

            var requestBody = "{ \"from\":\"0\", \"size\":\"30\",\"query\": { \"match\": { \"PdfText\": \"" + srch + "\"}}}";
            /*var requestBody = PostData.Serializable(
                                    new
                                    {
                                        from = 0,
                                        size = 30,
                                        query = new
                                        {
                                            match = new
                                            {
                                                PdfText = srch
                                            }
                                        }
                                    }); */



            try
            {
                var searchResponse = lowlevelClient.Search<StringResponse>(requestBody);
                Console.WriteLine("Search Finished");
                if (searchResponse.Success)
                {
                    var reponseJson = searchResponse.Body;

                    Console.Write(reponseJson);

                    var details = JObject.Parse(reponseJson);
                    var totalCnt = details["hits"]["hits"].Count();
                    for (int i = 0; i < totalCnt; i++)
                    {
                        Abschluse pdf = new Abschluse();
                        pdf.Id = (int)details["hits"]["hits"][i]["_id"];

                        pdf.PdfLink = details["hits"]["hits"][i]["_source"]["PdfLink"].ToString();
                        pdf.PdfText = details["hits"]["hits"][i]["_source"]["PdfText"].ToString();
                        pdf.DocumentUrl = details["hits"]["hits"][i]["_source"]["DocumentUrl"].ToString();

                        m_lstPdfInfos.Add(pdf);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }
        private void BeginResize1(object sender, PointerRoutedEventArgs e)
        {
#if !__WASM__
            var splitter = (UIElement)sender;
            if (!splitter.CapturePointer(e.Pointer))
            {
                return;
            }

            var capturedWidth = controlPane.ActualWidth;
            var capturedPoint = e.GetCurrentPoint(this).Position;
            var transform = splitter.RenderTransform as TranslateTransform
                ?? (TranslateTransform)(splitter.RenderTransform = new TranslateTransform());

            splitter.PointerMoved += Move;
            splitter.PointerReleased += Release;

			splitter.PointerCaptureLost += Lost;


            splitter.Opacity = .5;


            void Move(object o, PointerRoutedEventArgs args)
            {
                transform.X = args.GetCurrentPoint(this).Position.X - capturedPoint.X;
            }

            void Release(object o, PointerRoutedEventArgs args)
            {
                splitter.PointerMoved -= Move;
                splitter.PointerReleased -= Release;

				splitter.PointerCaptureLost -= Lost;


                controlPaneColumn.Width = new GridLength(capturedWidth + args.GetCurrentPoint(this).Position.X - capturedPoint.X);
                transform.X = 0;
                splitter.Opacity = 1;
            }

            void Lost(object o, PointerRoutedEventArgs args)
            {
                splitter.PointerMoved -= Move;
                splitter.PointerReleased -= Release;

				splitter.PointerCaptureLost -= Lost;


                transform.X = 0;
                splitter.Opacity = 1;
            }
#endif
        }
        private void BeginResize2(object sender, PointerRoutedEventArgs e)
        {
#if !__WASM__
            var splitter = (UIElement)sender;
            if (!splitter.CapturePointer(e.Pointer))
            {
                return;
            }

            var capturedWidth = resultPane.ActualWidth;
            var capturedPoint = e.GetCurrentPoint(this).Position;
            var transform = splitter.RenderTransform as TranslateTransform
                ?? (TranslateTransform)(splitter.RenderTransform = new TranslateTransform());

            Console.WriteLine("Begin: " + transform.X);
            splitter.PointerMoved += Move;
            splitter.PointerReleased += Release;

			splitter.PointerCaptureLost += Lost;


            splitter.Opacity = .5;


            void Move(object o, PointerRoutedEventArgs args)
            {
                transform.X = args.GetCurrentPoint(this).Position.X - capturedPoint.X;
                Console.WriteLine("Move: " + transform.X);
            }

            void Release(object o, PointerRoutedEventArgs args)
            {
                splitter.PointerMoved -= Move;
                splitter.PointerReleased -= Release;

				splitter.PointerCaptureLost -= Lost;

                resultPaneColumn.Width = new GridLength(capturedWidth + args.GetCurrentPoint(this).Position.X - capturedPoint.X);
                transform.X = 0;
                splitter.Opacity = 1;
                Console.WriteLine("Release: " + transform.X);
            }

            void Lost(object o, PointerRoutedEventArgs args)
            {
                splitter.PointerMoved -= Move;
                splitter.PointerReleased -= Release;

				splitter.PointerCaptureLost -= Lost;


                transform.X = 0;
                splitter.Opacity = 1;
            }
#endif
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Shared.AddElasticsearchService dialog = new Shared.AddElasticsearchService();
            
            await dialog.ShowAsync();
            if (Shared.MyResult.Add == dialog.Result)
            {
                ServiceInfo item = new ServiceInfo();

                item.URL = dialog.ServiceUrl;
                item.Index = dialog.Index;
                item.Type = dialog.Type;

                lstServiceInfos.Add(item);
            } 
        }
        private void AddButton_Click_wasm(object sender, RoutedEventArgs e)
        {
#if __WASM__
            if (w_index.Text == "" || w_type.Text == "" || w_serviceUrl.Text == "")
                return;

            ServiceInfo item = new ServiceInfo();

            item.URL = w_serviceUrl.Text;
            item.Index = w_index.Text;
            item.Type = w_type.Text;

            lstServiceInfos.Add(item);            
#endif
        }
        private void grid_SelectionChanged(object sender, object e) //DataGridSelectionChangedEventArgs e
        {
            
            Abschluse pdfInfo = (Abschluse)(myDataGrid.SelectedItem);
            Console.WriteLine("Selection Changed : " + pdfInfo);
            OutputPane.Text = pdfInfo.PdfText;
        }
    }
    public class ServiceInfo
    {
        public string URL { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
    }

    public class Abschluse
    {

        public int Id { get; set; }

        public string PdfLink { get; set; }

        public string PdfText { get; set; }

        public string DocumentUrl { get; set; }
    }
}
