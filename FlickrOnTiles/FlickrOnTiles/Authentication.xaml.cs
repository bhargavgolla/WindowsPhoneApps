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
using FlickrNet;

namespace FlickrOnTiles
{
    public partial class Authentication : PhoneApplicationPage
    {
        // A dummy callback url - as long as this is a valid URL it doesn't matter what it is
        private const string callbackUrl = "https://sites.google.com/site/oauthauthentication/";

        // The request token, held while the authentication is completed.
        private OAuthRequestToken requestToken = null;

        public OAuthAccessToken accessToken = null;

        public Authentication()
        {
            InitializeComponent();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Flickr f = FlickrManager.GetInstance();

            // obtain the request token from Flickr
            f.OAuthGetRequestTokenAsync(callbackUrl, r =>
            {
                // Check if an error was returned
                if (r.Error != null)
                {
                    Dispatcher.BeginInvoke(() => { MessageBox.Show("An error occurred getting the request token: " + r.Error.Message); });
                    return;
                }

                // Get the request token
                requestToken = r.Result;

                // get Authorization url
                string url = f.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Read);
                // Replace www.flickr.com with m.flickr.com for mobile version
                url = url.Replace("www.flickr.com", "m.flickr.com");

                // Navigate to url
                Dispatcher.BeginInvoke(() => { WebBrowser1.Navigate(new Uri(url)); });

            });
        }

        private void WebBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            // if we are not navigating to the callback url then authentication is not complete.
            if (!e.Uri.AbsoluteUri.StartsWith(callbackUrl)) return;

            // Get "oauth_verifier" part of the query string.
            var oauthVerifier = e.Uri.Query.Split('&')
                .Where(s => s.Split('=')[0] == "oauth_verifier")
                .Select(s => s.Split('=')[1])
                .FirstOrDefault();

            if (String.IsNullOrEmpty(oauthVerifier))
            {
                MessageBox.Show("Unable to find Verifier code in uri: " + e.Uri.AbsoluteUri);
                return;
            }

            // Found verifier, so cancel navigation
            e.Cancel = true;

            // Obtain the access token from Flickr
            Flickr f = FlickrManager.GetInstance();

            f.OAuthGetAccessTokenAsync(requestToken, oauthVerifier, r =>
            {
                // Check if an error was returned
                if (r.Error != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("An error occurred getting the access token: " + r.Error.Message);
                    });
                    return;
                }

                accessToken = r.Result;

                // Save the oauth token for later use
                FlickrManager.OAuthToken = accessToken.Token;
                FlickrManager.OAuthTokenSecret = accessToken.TokenSecret;
                FlickrManager.OAuthUserID = accessToken.UserId;

                Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Authentication completed for user " + accessToken.FullName + ", with token " + accessToken.Token);
                    NavigationService.Navigate(new Uri("/" + "MainPage" + ".xaml", UriKind.Relative));
                    return;
                });

            });

        }
    }
}