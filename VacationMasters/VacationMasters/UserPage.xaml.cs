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
using System.Threading.Tasks;
//import 
// using Microsoft.AspNet.Identity


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
           // Task.Run(() => Initialize());
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {Initialize(); });
        }

        private void Initialize()
        {
                FillCombo2();
                FillCombo1();
                FillText();
                FillPassword();
                FillConfirmPassword();

        }

            public static string UserName { set; get; }
            private void FillCombo2()
            {
                    // var Sql = string.Format("Select Category from Preferences  ");
                    var preferences= _dbWrapper.GetPreferences();
                    combo1.ItemsSource = preferences.ToArray();

            }

            private void FillCombo1()
            {
                combo2.ItemsSource = _dbWrapper.GetType();
            }
            public void FillText()
            {
                text_box_email.Text = _userManager.GetMail(UserName);
            }
            public void FillPassword()
            {
                password_box.Password = _userManager.GetPassword(UserName);
            }
            public void FillConfirmPassword()
            {
                confirm_password_box.Password = _userManager.GetPassword(UserName);
            }

            private void OrderHistory(object sender, RoutedEventArgs e)//OrderHistory
            {
                this.Frame.Navigate(typeof(OrderHistory), null);
            }
            private void Browse(object sender, RoutedEventArgs e)
            {

            }
            private void LogOut(object sender, RoutedEventArgs e)//LogOut
            {
                this.Frame.Navigate(typeof(MainPage), null);
            }
           

         
            private void SaveChanges(object sender, RoutedEventArgs e)
            {
                bool var;
                 
               if( radio_button.IsChecked == true)

                     var = true;
               else 
                    var = false;
                    /*\
                     strCurrentUserId = User.Identity.GetUserId();
 
                     */
                _userManager.UpdateUser(
                    UserName,
                    var,
                    text_box_email.Text,
                    password_box.Password,
                    confirm_password_box.Password,
                    combo1.SelectedValue != null ? combo1.SelectedValue.ToString() : string.Empty,
                    combo2.SelectedValue != null ? combo2.SelectedValue.ToString() : string.Empty);

            }


        }
    }

