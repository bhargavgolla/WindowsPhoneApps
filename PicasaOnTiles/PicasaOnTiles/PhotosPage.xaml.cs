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
using System.IO.IsolatedStorage;
using System.Collections;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

namespace PicasaOnTiles
{
    public partial class PhotosPage : PhoneApplicationPage
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

        //public AlbumImages albumImages = new AlbumImages();
        public int selectedImageIndex;
        public int selectedAlbumIndex;
        public string auth;
        private int start = -7;
        private int panel_count = 0;

        public PhotosPage()
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // load saved username from isolated storage
            if (appSettings.Contains(usernameKey))
            {
                string differentUsername = (string)appSettings[usernameKey];
                if (differentUsername != "") username = differentUsername;
            }

            // load saved email from isolated storage
            if (appSettings.Contains(emailKey))
            {
                email = (string)appSettings[emailKey]; // for example firstname.lastname@gmail.com
                if (username == "" && email.IndexOf("@") != -1) username = "default"; // username = email.Substring(0, email.IndexOf("@")); firstname.lastname
                String dataFeed = String.Format("http://picasaweb.google.com/data/feed/api/user/{0}?alt=json", username);
            }

            // load password from isolated storage
            if (appSettings.Contains(passwordKey))
            {
                password = (string)appSettings[passwordKey];
            }

            // we are coming back from AlbumPage
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                //AlbumsListBox.ItemsSource = app.albums;
                //AlbumsListBox.SelectedIndex = -1;
            }
            else
            {
                // get authentication from Google
                GetAuth();
            }
        }

        private void GetAuth()
        {
            string service = "lh2"; // Picasa
            string accountType = "GOOGLE";

            WebClient webClient = new WebClient();
            Uri uri = new Uri(string.Format("https://www.google.com/accounts/ClientLogin?Email={0}&Passwd={1}&service={2}&accountType={3}", email, password, service, accountType));
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(AuthDownloaded);
            webClient.DownloadStringAsync(uri);
        }

        private void AuthDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Error == null)
                {
                    auth = "";
                    int index = e.Result.IndexOf("Auth=");
                    if (index != -1)
                    {
                        auth = e.Result.Substring(index + 5);
                    }
                    if (auth != "")
                    {
                        // get albums from Google
                        GetImages();
                        return;
                    }
                }
                MessageBox.Show("Cannot get authentication from google. Check your login and password.");
            }
            catch (WebException)
            {
                MessageBox.Show("Cannot get authentication from google. Check your login and password.");
            }
        }

        private void GetImages()
        {
            start += 8;
            if ((string)appSettings[usernameKey] == "")
            {
                username = "default";
            }
            else
            {
                username = (string)appSettings[usernameKey];
            }
            String dataFeed = String.Format("https://picasaweb.google.com/data/feed/api/user/{0}?alt=json&kind=photo&thumbsize=160c&max-results=8&start-index={1}&fields=link[@rel=%22next%22],entry(summary,link[@rel=%22alternate%22],media:group(media:thumbnail),gphoto:id,gphoto:albumid)", username, start);
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = "GoogleLogin auth=" + auth;
            Uri uri = new Uri(dataFeed, UriKind.Absolute);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ImagesDownloaded);
            webClient.DownloadStringAsync(uri);
        }

        public void ImagesDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null || e.Error != null)
                {
                    MessageBox.Show("Cannot get albums data from Picasa server!");
                    return;
                }
                else
                {
                    panel_count++;
                    Grid grid = new Grid();
                    grid.Name = "Grid" + panel_count;
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(200)});
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
                    stackPanel1.Children.Add(grid);
                    // Deserialize JSON string to dynamic object
                    IDictionary<string, object> json = (IDictionary<string, object>)SimpleJson.DeserializeObject(e.Result);
                    // Feed object
                    IDictionary<string, object> feed = (IDictionary<string, object>)json["feed"];
                    // Image entries
                    IList entries = (IList)feed["entry"];
                    // Delete previous albums from albums list
                    //albumImages.Clear();
                    // Find album details
                    for (int i = 0; i < entries.Count; i++)
                    {
                        // Create a new Album
                        AlbumImage albumImage = new AlbumImage();
                        // Album entry object
                        IDictionary<string, object> entry = (IDictionary<string, object>)entries[i];
                        // Published object
                        IDictionary<string, object> summary = (IDictionary<string, object>)entry["summary"];
                        // Image Caption
                        if ((string)summary["$t"] == "")
                        {
                            albumImage.title = "No caption";
                        }
                        else
                        {
                            albumImage.title = (string)summary["$t"];
                        }
                        // Link List
                        IList link = (IList)entry["link"];
                        // First link is Image URL in Picasa
                        IDictionary<string, object> href = (IDictionary<string, object>)link[0];
                        // Get image webURL
                        albumImage.webUrl = (string)href["href"];
                        // Media group object
                        IDictionary<string, object> mediagroup = (IDictionary<string, object>)entry["media$group"];
                        // Media thumbnail object list
                        IList mediathumbnailList = (IList)mediagroup["media$thumbnail"];
                        // First thumbnail object (smallest)
                        IDictionary<string, object> mediathumbnail = (IDictionary<string, object>)mediathumbnailList[0];
                        // Get thumbnail url
                        albumImage.thumbnail = (string)mediathumbnail["url"];
                        IDictionary<string, object> photoid = (IDictionary<string, object>)entry["gphoto$id"];
                        IDictionary<string, object> photoalbumid = (IDictionary<string, object>)entry["gphoto$albumid"];
                        albumImage.content = GetComments((string)photoid["$t"], (string)photoalbumid["$t"]);
                        HubTile hubtile = new HubTile();
                        hubtile.Title = albumImage.title;
                        hubtile.Source = new BitmapImage(new Uri(albumImage.thumbnail));
                        hubtile.Name = albumImage.webUrl;// +":" + (string)photoid["$t"];
                        hubtile.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(hubtile_Tap);
                        hubtile.Message = albumImage.content;
                        hubtile.SetValue(Grid.RowProperty, i / 2);
                        hubtile.SetValue(Grid.ColumnProperty, i % 2);
                        grid.Children.Add(hubtile);
                        /*if (i == entries.Count - 1)
                        {
                            int newCount = 0;
                            String imageURL = albumImage.thumbnail;
                            // Application Tile is always the first Tile, even if it is not pinned to Start.
                            ShellTile TileToFind = ShellTile.ActiveTiles.First();
                            // Application should always be found
                            if (TileToFind != null)
                            {
                                // Set the properties to update for the Application Tile.
                                // Empty strings for the text values and URIs will result in the property being cleared.
                                StandardTileData NewTileData = new StandardTileData
                                {
                                    Title = "Picasa-On-Tiles",
                                    BackgroundImage = new Uri(imageURL,UriKind.Absolute),
                                    //BackgroundImage = new Uri((string)mediathumbnail["url"]),
                                    //BackgroundImage = ((System.Windows.Media.Imaging.BitmapImage)hubtile.Source).UriSource,
                                    Count = newCount,
                                    BackTitle = "What is This?",
                                    BackContent = "Picasa-on-Tiles. An app for photofreaks"
                                };

                                // Update the Application Tile
                                TileToFind.Update(NewTileData);
                            }
                        }*/
                    }
                    /*if (entries.Count != 8)
                    {
 
                    }*/
                    // Add albums to AlbumListBox
                    //AlbumsListBox.ItemsSource = app.albums;
                }
            }
            catch (WebException)
            {
                MessageBox.Show("Cannot get albums data from Picasa server!");
            }
            catch (KeyNotFoundException)
            {
                //MessageBox.Show("Cannot load images from Picasa Server - JSON parsing error happened!");
            }
        }

        void hubtile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile tapped = (HubTile)sender;
            //NavigationService.Navigate(new Uri(tapped.Name, UriKind.Absolute));

            WebBrowserTask task = new WebBrowserTask { Uri = new Uri(tapped.Name) };
            task.Show();
        }

        public String GetComments(String photoID, String AlbumID) 
        {
            String comments = "Hello Boss";
            if ((string)appSettings[usernameKey] == "")
            {
                username = "default";
            }
            else
            {
                username = (string)appSettings[usernameKey];
            }
            String dataFeed = String.Format("https://picasaweb.google.com/data/feed/api/user/{0}/albumid/{1}/photoid/{2}?kind=comment&alt=json&fields=entry(title,content)", username,AlbumID,photoID);
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = "GoogleLogin auth=" + auth;
            Uri uri = new Uri(dataFeed, UriKind.Absolute);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(uri);
            return comments;
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null || e.Error != null)
                {
                    MessageBox.Show("Cannot get albums data from Picasa server!");
                }
                else
                {
                    String comments = "";
                    // Deserialize JSON string to dynamic object
                    IDictionary<string, object> json = (IDictionary<string, object>)SimpleJson.DeserializeObject(e.Result);
                    // Feed object
                    IDictionary<string, object> feed = (IDictionary<string, object>)json["feed"];
                    // Comment entries
                    IList entries = (IList)feed["entry"];
                    for (int i = 0; i < entries.Count; i++) 
                    {
                        // Album entry object
                        IDictionary<string, object> entry = (IDictionary<string, object>)entries[i];
                        // Published object
                        IDictionary<string, object> title = (IDictionary<string, object>)entry["title"];
                        comments = comments+(string)title["$t"]+": ";
                        // Published object
                        IDictionary<string, object> comment = (IDictionary<string, object>)entry["content"];
                        comments = comments + (string)comment["$t"] + " ";
                    }
                    if (comments == "")
                        comments = "No Comments";
                    //return comments;

                }
            }
            catch (WebException)
            {
                MessageBox.Show("Cannot get albums data from Picasa server!");
            }
            catch (KeyNotFoundException)
            {
                //MessageBox.Show("Cannot load images from Picasa Server - JSON parsing error happened!");
            }
        }
    }
}