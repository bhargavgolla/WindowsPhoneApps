﻿#pragma checksum "C:\Users\Bhargav\Documents\GitHub\WindowsPhone-PhoneGap\RFCReader-Panaroma\RFCReader-Panaroma\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "53BA2C32DBD3AD491B4C8B93B17055CA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace RFCReader_Panaroma {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock textBlock1;
        
        internal System.Windows.Controls.TextBox textBox1;
        
        internal System.Windows.Controls.Button button1;
        
        internal Microsoft.Phone.Controls.WebBrowser webBrowser1;
        
        internal System.Windows.Controls.StackPanel History;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/RFCReader;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.textBlock1 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock1")));
            this.textBox1 = ((System.Windows.Controls.TextBox)(this.FindName("textBox1")));
            this.button1 = ((System.Windows.Controls.Button)(this.FindName("button1")));
            this.webBrowser1 = ((Microsoft.Phone.Controls.WebBrowser)(this.FindName("webBrowser1")));
            this.History = ((System.Windows.Controls.StackPanel)(this.FindName("History")));
        }
    }
}

