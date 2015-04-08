using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VacationMasters.Wrappers;
using VacationMasters.UserManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VacationMasters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private IDbWrapper _dbWrapper;
        private IUserManager _userManager;

        public UserPage()
        {
            this.InitializeComponent();
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            FillCombo2();
            FillCombo1();
        }
       
             
        private void FillCombo2()
        {
            // var Sql = string.Format("Select Category from Preferences  ");
            combo1.ItemsSource = _dbWrapper.GetPreferences();

        }

        private void FillCombo1()
        {
            combo2.ItemsSource = _dbWrapper.GetType();
        }


        private void home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }
        private void packages_Click(object sender, RoutedEventArgs e)
        {

        }
        private void user_panel_Click(object sender, RoutedEventArgs e)
        {

        }
        private void contact_Click(object sender, RoutedEventArgs e)
        {

        }
        private void admin_control_Click(object sender, RoutedEventArgs e)
        {

        }
        private void order_history_Click(object sender, RoutedEventArgs e)
        {

        }
        private void browse_Click(object sender, RoutedEventArgs e)
        {

        }
        private void log_out_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }
        public static string UserName { set; get; }


        private void save_Click(object sender, RoutedEventArgs e)
        {


            _userManager.UpdateUser(
                UserName, 
                text_box_email.Text, 
                password_box.ToString(), 
                confirm_password_box.ToString(), 
                combo1.SelectedValue!=null?combo1.SelectedValue.ToString():string.Empty, 
                combo2.SelectedValue!=null?combo2.SelectedValue.ToString():string.Empty);

        }
       
        
    }
}
