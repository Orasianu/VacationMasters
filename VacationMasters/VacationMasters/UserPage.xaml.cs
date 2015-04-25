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
            Task.Run(() => Initialize());    
        }

        private void Initialize()
        {
            try
            {
                FillCombo2();
                FillCombo1();
            }
            catch (Exception e) { }
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

            private void order_history_Click(object sender, RoutedEventArgs e)//OrderHistory
            {
                this.Frame.Navigate(typeof(OrderHistory), null);
            }
            private void browse_Click(object sender, RoutedEventArgs e)
            {

            }
            private void log_out_Click(object sender, RoutedEventArgs e)//LogOut
            {
                this.Frame.Navigate(typeof(MainPage), null);
            }
            public static string UserName { set; get; }


            private void save_Click(object sender, RoutedEventArgs e)
            {
                bool var;
                 
               if( radio_button.IsChecked == true)

                     var = true;
               else 
                    var = false;

                _userManager.UpdateUser(
                    UserName,
                    var,
                    text_box_email.Text,
                    password_box.ToString(),
                    confirm_password_box.ToString(),
                    combo1.SelectedValue != null ? combo1.SelectedValue.ToString() : string.Empty,
                    combo2.SelectedValue != null ? combo2.SelectedValue.ToString() : string.Empty);

            }


        }
    }

