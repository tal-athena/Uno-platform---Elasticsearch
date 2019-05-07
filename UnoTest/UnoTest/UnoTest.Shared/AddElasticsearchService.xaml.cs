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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoTest.Shared
{
    public enum MyResult
    {
        Add,        
        Cancle,
        Nothing
    }
    public sealed partial class AddElasticsearchService : ContentDialog
    {
        public MyResult Result { get; set; }
        public string ServiceUrl { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }

        public AddElasticsearchService()
        {
            this.InitializeComponent();
            this.Result = MyResult.Nothing;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            if (serviceUrl.Text == "" || index.Text == "" || type.Text == "")
                return;

            this.ServiceUrl = serviceUrl.Text;
            this.Index = index.Text;
            this.Type = type.Text;

            this.Result = MyResult.Add;

            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = MyResult.Cancle;

            this.Hide();
        }
    }
}
