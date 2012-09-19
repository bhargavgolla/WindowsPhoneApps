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
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;

namespace WristWatch
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 1;
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

            /*MarketPlace Icon*/
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                button3.IconUri = new Uri("/Images/dark/appbar.favs.rest.png", UriKind.Relative);
            }
            else
            {
                button3.IconUri = new Uri("/Images/light/appbar.favs.rest.png", UriKind.Relative);
            }
            button3.Text = "Rate";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(market_Click);

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            menuItem1.Text = "Share this app with your Friends";
            ApplicationBar.MenuItems.Add(menuItem1);

            /*Wrist Watch Component*/
            string url = "http://codepen.io/bhargavgolla/full/Ldbkr";
            
            webBrowser1.Navigate(new Uri(url, UriKind.Absolute));
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: WristWatch??";
            task.Body = "Hey check out this great WP App named WristWatch. You can find it on http://www.windowsphone.com/s?appid=de173b7a-0aff-4812-9d18-d7bf8ee15774";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=de173b7a-0aff-4812-9d18-d7bf8ee15774", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: WristWatch??";
            shareLinkTask.Show();
        }

        private void market_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}