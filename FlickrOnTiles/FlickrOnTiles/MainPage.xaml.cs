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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using FlickrNet;
using System.Windows.Media.Imaging;
using System.Net.NetworkInformation;

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

        private void market_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void flickrAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            // Create Flickr instance
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NavigationService.Navigate(new Uri("/" + "Authentication" + ".xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Active internet Connection is needed to perform this action."); 
            }
            
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
                            hubTile.Source = new BitmapImage(new Uri(photos[count].SquareThumbnailUrl));
                            hubTile.Name = photos[count].WebUrl;
                            hubTile.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(hubTile_Tap);
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
                                String imageURL = photos[count-1].SquareThumbnailUrl;
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

        void hubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile tapped = (HubTile)sender;
            //NavigationService.Navigate(new Uri(tapped.Name, UriKind.Absolute));
            WebBrowserTask task = new WebBrowserTask { Uri= new Uri(tapped.Name)};
            task.Show();
        }
    }
}