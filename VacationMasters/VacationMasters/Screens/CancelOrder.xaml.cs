using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VacationMasters.Screens
{
    public sealed partial class CancelOrder : UserControl, INotifyPropertyChanged
    {
        private bool _isOperationInProgress;

        private UserManager _userManager;

        private DbWrapper _dbWrapper;
        public DependencyProperty dp { get; set; }

        public CancelOrder()
        {
            this.DataContext = this;
            this.InitializeComponent();
            FillOrders();

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsOperationInProgress
        {
            get { return _isOperationInProgress; }
            set
            {
                if (value != _isOperationInProgress)
                {
                    _isOperationInProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private async void FillOrders()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            List<string> orderUser = _userManager.GetOrders("Orasianu");
            foreach (var i in orderUser)
            {
                OrdersGridView.SetValue(dp, i);
            }
          
            foreach (string j in OrdersGridView.Items)
            {
                var list = _userManager.GetPackagesCommmand(j);
                foreach (string i in list)
                {
                    PackagesGridView.SetValue(dp,i);
                }
            }
            IsOperationInProgress = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content;
            VisualStateManager.GoToState(page, "UserPageControl", true);
        }



       
    }
}
