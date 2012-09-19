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
using FlickrNet;
using System.Windows.Media.Imaging;

namespace FlickrOnTiles
{
    public partial class MainPage : PhoneApplicationPage
    {
        public int page = 1;
        public int perPage = 8;
        public int app_count = 0;
        public int panel_count = 0;
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

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            menuItem1.Text = "Share this app with your Friends";
            ApplicationBar.MenuItems.Add(menuItem1);
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: Flickr-On-Tiles??";
            task.Body = "Hey check out this great WP App named Flickr-On-Tiles. You can find it on http://www.windowsphone.com/s?appid=ece71322-d6c0-42bc-92b3-04e4bc55a2d3";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=ece71322-d6c0-42bc-92b3-04e4bc55a2d3", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: Flickr-On-Tiles??";
            shareLinkTask.Show();
        }

        private void flickrAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            // Create Flickr instance
            NavigationService.Navigate(new Uri("/" + "Authentication" + ".xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*Button morebtn = (Button)this.FindName("moreButton");
            stackPanel1.Children.Remove(morebtn);*/
            if (app_count == 0)
            {
                Button more_btn = new Button();
                more_btn.Content = "More...";
                more_btn.Name = "MoreButton";
                more_btn.Click += PhoneApplicationPage_Loaded;
                stackPanel1.Children.Add(more_btn);
                app_count++;
            }
            Button morebtn = (Button)this.FindName("MoreButton");
            stackPanel1.Children.Remove(morebtn);
            if (FlickrManager.OAuthToken == null || FlickrManager.OAuthTokenSecret == null)
                return;
            else
            {
                int count=0;
                Grid grid = new Grid();
                grid.Name = "Grid" + panel_count;
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(200)});
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                stackPanel1.Children.Add(grid);
                //scrollViewer1.Content = grid;
                Button btn = (Button)this.FindName("flickrAuthenticate");
                btn.Content = "Not your Account?";
                Flickr f = FlickrManager.GetAuthInstance();

                f.PeopleGetPhotosAsync(FlickrManager.OAuthUserID, page, perPage, r => 
                {
                    if (r.Error != null)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("An error occurred talking to Flickr: " + r.Error.Message);
                        });
                        return;
                    }

                    PhotoCollection photos = r.Result;

                    Dispatcher.BeginInvoke(() =>
                    {
                        //ResultsListBox.ItemsSource = photos;
                        while (count != photos.Count)
                        {
                            int comments_count=0;
                            String full_comment = "";

                            HubTile hubTile = new HubTile();
                            hubTile.Title = photos[count].Title;
                            hubTile.Source = new BitmapImage(new Uri(photos[count].ThumbnailUrl));
                            Flickr flickr = FlickrManager.GetAuthInstance();
                            flickr.PhotosCommentsGetListAsync(photos[count].PhotoId, res => 
                            {
                                if (res.Error != null)
                                {
                                    Dispatcher.BeginInvoke(() =>
                                    {
                                        MessageBox.Show("An error occurred talking to Flickr: " + res.Error.Message);
                                    });
                                    return;
                                }
                                PhotoCommentCollection comments = res.Result;
                                if (comments.Count == 0)
                                    full_comment = "No comments";
                                else
                                {
                                    while (comments_count != comments.Count)
                                    {
                                        full_comment = full_comment + comments[comments_count].AuthorUserName + ":" + comments[comments_count].CommentHtml + " ";
                                        comments_count++;
                                    }
                                }
                                hubTile.Message = full_comment;
                            });
                            //stackPanel1.Children.Add(comment);
                            //stackPanel1.Children.Add(hubTile);
                            hubTile.SetValue(Grid.RowProperty, count / 2);
                            hubTile.SetValue(Grid.ColumnProperty, count % 2);
                            grid.Children.Add(hubTile);
                            count++;
                            if (count == photos.Count)
                            {
                                int newCount = 0;
                                String imageURL = photos[count-1].ThumbnailUrl;
                                // Application Tile is always the first Tile, even if it is not pinned to Start.
                                ShellTile TileToFind = ShellTile.ActiveTiles.First();
                                // Application should always be found
                                if (TileToFind != null)
                                {
                                    // Set the properties to update for the Application Tile.
                                    // Empty strings for the text values and URIs will result in the property being cleared.
                                    StandardTileData NewTileData = new StandardTileData
                                    {
                                        Title = "Flickr-On-Tiles",
                                        BackgroundImage = new Uri(imageURL),
                                        Count = newCount,
                                        BackTitle = "What is This?",
                                        BackContent = "Flickr-on-Tiles. An app for photofreaks"
                                    };

                                    // Update the Application Tile
                                    TileToFind.Update(NewTileData);
                                }
                            }
                        }
                        if (photos.Pages != page)
                        {
                            page++;
                            stackPanel1.Children.Add(morebtn);
                        }
                    });
                    return;
                });
            }
        }
    }
}