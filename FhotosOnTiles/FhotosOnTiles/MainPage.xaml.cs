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
using Facebook;
using System.Collections;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

namespace FhotosOnTiles
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string _accessToken="";
        private int count = 0;
        private int panel_count = 0;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: Fhotos-On-Tiles??";
            task.Body = "Hey check out this great WP App named Flickr-On-Tiles. You can find it on http://www.windowsphone.com/s?appid=68f7afd5-0d71-4006-87da-4d75b4d919a8";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=68f7afd5-0d71-4006-87da-4d75b4d919a8", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: Fhotos-On-Tiles??";
            shareLinkTask.Show();
        }

        private void market_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void facebookLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/" + "FacebookLoginPage" + ".xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                _accessToken = NavigationContext.QueryString["access_token"];
                while (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }
            catch (KeyNotFoundException) { }
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_accessToken != "")
            {
                facebookLogin.Content = "Try another account?";
                GraphApiPhotos();
                if (this.FindName("MoreButton")!=null)
                {
                    Button morebtn = (Button)this.FindName("MoreButton");
                    stackPanel1.Children.Remove(morebtn);
                }
            }
        }

        private void GraphApiPhotos()
        {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }
                count =count+ 8;
                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                {
                    int i;
                    panel_count++;
                    Grid grid = new Grid();
                    grid.Name = "Grid" + panel_count;
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    stackPanel1.Children.Add(grid);
                    IList data = (IList)result["data"];
                    for (i = 0; i < data.Count; i++)
                    {
                        IDictionary<string, object> entry = (IDictionary<string, object>)data[i];
                        HubTile hubTile = new HubTile();
                        try
                        {
                            hubTile.Title = (string)entry["name"];
                        }
                        catch
                        {
                            hubTile.Title = "No caption";
                        }
                        //hubTile.Title = "Title";
                        hubTile.Source = new BitmapImage(new Uri((string)entry["picture"]));
                        hubTile.Name = (string)entry["link"];
                        GetComments((string)entry["id"], (string)entry["link"]);
                        hubTile.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(hubTile_Tap);
                        hubTile.SetValue(Grid.RowProperty, i / 2);
                        hubTile.SetValue(Grid.ColumnProperty, i % 2);
                        grid.Children.Add(hubTile);
                        if (i == data.Count - 1)
                        {
                            int newCount = 0;
                            String imageURL = (string)entry["picture"];
                            String imageURI = imageURL.Replace("https://", "http://");
                            // Application Tile is always the first Tile, even if it is not pinned to Start.
                            ShellTile TileToFind = ShellTile.ActiveTiles.First();
                            // Application should always be found
                            if (TileToFind != null)
                            {
                                // Set the properties to update for the Application Tile.
                                // Empty strings for the text values and URIs will result in the property being cleared.
                                StandardTileData NewTileData = new StandardTileData
                                {
                                    Title = "Fhotos-On-Tiles",
                                    //BackgroundImage = new Uri(imageURL, UriKind.Absolute),
                                    BackgroundImage = new Uri(imageURI),
                                    Count = newCount,
                                    BackTitle = "What is This?",
                                    BackContent = "View your Facebook photos on tiles."
                                };

                                // Update the Application Tile
                                TileToFind.Update(NewTileData);
                            }
                        }
                    }
                    if (i == 8)
                    {
                        Button more_btn = new Button();
                        more_btn.Content = "More...";
                        more_btn.Name = "MoreButton";
                        more_btn.Click += PhoneApplicationPage_Loaded;
                        stackPanel1.Children.Add(more_btn);
                    }

                });
            };

            fb.GetAsync("me/photos?limit=8&offset=" + count + "&fields=name,picture,link");
        }

        void hubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile tapped = (HubTile)sender;
            WebBrowserTask task = new WebBrowserTask { Uri = new Uri(tapped.Name) };
            task.Show();
        }
        private void GetComments(String photoID,String URL)
        {
            String comments = "";
            var fb = new FacebookClient(_accessToken);
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }
                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                {
                    int min=2;
                    IList data = (IList)result["data"];
                    if (data.Count < 2)
                        min = data.Count;
                    for (int i = 0; i < min; i++)
                    {
                        IDictionary<string, object> entry = (IDictionary<string, object>)data[i];
                        IDictionary<string, object> from = (IDictionary<string, object>)entry["from"];
                        comments = comments + (string)from["name"] + ": ";
                        comments = comments + (string)entry["message"]+ " ";
                    }
                    if (comments == "")
                        comments = "No comments. Tap to open in facebook and comment there.";
                    HubTile hubtile = (HubTile)this.FindName(URL);
                    hubtile.Message = comments;

                });
            };
            fb.GetAsync(photoID + "/comments?limit=2&fields=from,message");
        }

    }
}