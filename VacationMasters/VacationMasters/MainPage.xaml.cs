﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VacationMasters.Essentials;
using VacationMasters.Wrappers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VacationMasters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public DbWrapper DbWrapper { get; set; }

        public MainPage()
        {
           /* this.DbWrapper = new DbWrapper();
            var packageList = this.DbWrapper.GetAllPackages();*/
            this.InitializeComponent();
                
            

        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private void packages_Click(object sender, RoutedEventArgs e)
        {
            // this.Frame.Navigate(typeof(Packages), null);

        }

        private void user_panel_Click(object sender, RoutedEventArgs e)
        {
             this.Frame.Navigate(typeof(UserPage), null);

        }

        private void admin_control_Click(object sender, RoutedEventArgs e)
        {
            // this.Frame.Navigate(typeof(AdminControl), null);
        }

        private void contact_Click(object sender, RoutedEventArgs e)
        {
            // this.Frame.Navigate(typeof(Contact), null);
        }


        private void search_Click(object sender, RoutedEventArgs e)
        {


        }



    }
}
