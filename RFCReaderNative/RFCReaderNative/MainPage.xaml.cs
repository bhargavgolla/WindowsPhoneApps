using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace RFCReaderNative
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string site;
            site = textBox1.Text;
            if (site.Length!=4)
                MessageBox.Show("RFC Doc ID has to be 4 digit. Please enter the correct ID");
            else
            {
                site = "http://docs.google.com/viewer?url=http://tools.ietf.org/pdf/rfc" + site + ".pdf";
                webBrowser1.Navigate(new Uri(site, UriKind.Absolute));
            }
        }
        
    }
}