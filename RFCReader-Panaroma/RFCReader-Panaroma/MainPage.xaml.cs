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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace RFCReader_Panaroma
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.4;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            /*Mail AppBar*/
            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                button1.IconUri = new Uri("/Images/dark/appbar.feature.email.rest.png", UriKind.Relative);
            }
            else
            {
                button1.IconUri = new Uri("/Images/light/appbar.feature.email.rest.png", UriKind.Relative);
            }
            button1.Text = "Mail";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(email_Click);

            /*Facebook Appbar*/
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                button2.IconUri = new Uri("/Images/dark/appbar.share.rest.png", UriKind.Relative);
            }
            else
            {
                button2.IconUri = new Uri("/Images/light/appbar.share.rest.png", UriKind.Relative);
            }
            button2.Text = "Share";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(fb_Click);

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            menuItem1.Text = "Share this app with your Friends";
            ApplicationBar.MenuItems.Add(menuItem1);
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
            if (site.Length!= 4)
                MessageBox.Show("RFC Doc ID has to be 4 digit. Please enter the correct ID");
            else
            {
                string url = "http://docs.google.com/viewer?url=http://tools.ietf.org/pdf/rfc" + site + ".pdf";
                webBrowser1.Navigate(new Uri(url, UriKind.Absolute));
                TextBlock link = new TextBlock();
                link.Text = "RFC doc" + site;
                this.History.Children.Add(link);
            }
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: RFC Reader??";
            task.Body = "Hey check out this great WP App named RFC Reader. You can find it on http://www.windowsphone.com/s?appid=1d8b6bcf-af39-4643-b971-f33d4505bf92";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=1d8b6bcf-af39-4643-b971-f33d4505bf92", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: RFC Reader??";
            shareLinkTask.Show();
        }
    }
}