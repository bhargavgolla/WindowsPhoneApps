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
using System.IO.IsolatedStorage;

namespace PicasaOnTiles
{
    public partial class MainPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings;
        private const string emailKey = "emailKey";
        private const string passwordKey = "passwordKey";
        private const string savepasswordKey = "savepasswordKey";
        private const string usernameKey = "usernameKey";

        private string username = "";
        private string email = "";
        private string password = "";
        private bool savepassword = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            appSettings = IsolatedStorageSettings.ApplicationSettings;
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: Picasa-On-Tiles??";
            task.Body = "Hey check out this great WP App named Picasa-On-Tiles. You can find it on marketplace here, http://www.windowsphone.com/s?appid=d82403d1-dc31-487f-8865-c308f143403f";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=d82403d1-dc31-487f-8865-c308f143403f", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: Picasa-On-Tiles??";
            shareLinkTask.Show();
        }

        private void market_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            username = UsernameTextBox.Text;
            email = EmailTextBox.Text;
            password = PasswordTextBox.Password;
            savepassword = (bool)SavePasswordCheckBox.IsChecked;
            this.NavigationService.Navigate(new Uri("/PhotosPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // load and show saved email from isolated storage
            if (appSettings.Contains(emailKey))
            {
                email = (string)appSettings[emailKey];
            }
            EmailTextBox.Text = email;

            // load password from isolated storage
            if (appSettings.Contains(passwordKey))
            {
                password = (string)appSettings[passwordKey];
            }

            // username from isolated storage
            if (appSettings.Contains(usernameKey))
            {
                username = (string)appSettings[usernameKey];
            }
            UsernameTextBox.Text = username;

            // show password if selected to save
            if (appSettings.Contains(savepasswordKey))
            {
                string savepass = (string)appSettings[savepasswordKey];
                if (savepass == "true")
                {
                    savepassword = true;
                    PasswordTextBox.Password = password;
                }
                else
                {
                    savepassword = false;
                    PasswordTextBox.Password = "";
                }
                SavePasswordCheckBox.IsChecked = savepassword;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // add email to isolated storage
            appSettings.Remove(emailKey);
            appSettings.Add(emailKey, email);

            // add savepassword and password to isolated storage
            appSettings.Remove(savepasswordKey);
            appSettings.Remove(passwordKey);

            // add username to isolated storage
            appSettings.Remove(usernameKey);
            appSettings.Add(usernameKey, username);

            if (SavePasswordCheckBox.IsChecked == true)
            {
                appSettings.Add(savepasswordKey, "true");
                appSettings.Add(passwordKey, password);
            }
            else
            {
                appSettings.Add(savepasswordKey, "false");
                appSettings.Add(passwordKey, password);
            }
        }
        
    }
}