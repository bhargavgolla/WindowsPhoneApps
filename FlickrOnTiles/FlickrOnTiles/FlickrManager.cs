using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FlickrNet;
using System.IO.IsolatedStorage;

namespace FlickrOnTiles
{
    public class FlickrManager
    {
        public const string ApiKey = "d228130f186f94d6373be28ed078b282";
        public const string SharedSecret = "f92f3d325b890f22";

        public static Flickr GetInstance()
        {
            return new Flickr(ApiKey, SharedSecret);
        }

        public static Flickr GetAuthInstance()
        {
            var f = new Flickr(ApiKey, SharedSecret);
            f.OAuthAccessToken = OAuthToken;
            f.OAuthAccessTokenSecret = OAuthTokenSecret;
            return f;
        }

        public static string OAuthToken
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OAuthToken"))
                    return IsolatedStorageSettings.ApplicationSettings["OAuthToken"] as string;
                else
                    return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OAuthToken"] = value;
            }
        }

        public static string OAuthTokenSecret
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OAuthTokenSecret"))
                    return IsolatedStorageSettings.ApplicationSettings["OAuthTokenSecret"] as string;
                else
                    return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OAuthTokenSecret"] = value;
            }
        }
        public static string OAuthUserID
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OAuthUserID"))
                    return IsolatedStorageSettings.ApplicationSettings["OAuthUserID"] as string;
                else
                    return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OAuthUserID"] = value;
            }
        }
    }
}
