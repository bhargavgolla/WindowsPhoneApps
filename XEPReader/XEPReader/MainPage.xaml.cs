﻿using System;
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
using Microsoft.Phone.Tasks;

namespace XEPReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string site;
            site = textBox1.Text;
            if (site.Length != 4)
                MessageBox.Show("XEP Doc ID has to be 4 digit. Please enter the correct ID");
            else
            {
                string url = "http://docs.google.com/viewer?url=http://xmpp.org/extensions/xep-" + site + ".pdf";
                webBrowser1.Navigate(new Uri(url, UriKind.Absolute));
                TextBlock link = new TextBlock();
                link.Text = "XEP doc" + site;
                this.History.Children.Add(link);
            }
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: XEP Reader??";
            task.Body = "Hey check out this great WP App named XEP Reader. You can find it on http://www.windowsphone.com/s?appid=6452cf16-c10d-4f2f-9412-0ecc320b6a61";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=6452cf16-c10d-4f2f-9412-0ecc320b6a61", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: XEP Reader??";
            shareLinkTask.Show();
        }

        private void market_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}