using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using VacationMasters.Essentials;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VacationMasters.Screens
{
    public sealed partial class UserPage : UserControl, INotifyPropertyChanged
    {
        private DbWrapper _dbWrapper ;
        private UserManager _userManager;
        private GroupManager _groupManager;
         private byte[] _image;

        private bool _isOperationInProgress;
        public UserPage()
        {
            this.DataContext = this;
            InitializeComponent();
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            
           
            FillPreferencesCountry();
            FillPreferencesType();
            FillGroups();
            FillRadioButton();
           

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { Initialize(); });
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

        private async void FillPreferencesCountry()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            var preferences = await Task.Run(() => _dbWrapper.GetAllPreferences());
           // var preferencesUseit Task.Run(() =>  _userManager.GetPreferencesCountryUser(UserName));
            List<string> preferencesUser = _userManager.GetPreferencesCountryUser("Orasianu");
            CountriesGridView.ItemsSource = preferences.Where(c => c.Category == "Country").Select(d => d.Name).ToArray();
           foreach (string i in preferencesUser)
            {
                if (CountriesGridView.Items != null)
                    foreach ( var j in CountriesGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            CountriesGridView.SelectedItems.Add(j);
                        }
                    }
            }
            
            IsOperationInProgress = false;
        }

    

        private async void FillPreferencesType()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            var preferences = await Task.Run(() => _dbWrapper.GetAllPreferences());
            List<string> preferencesUser = _userManager.GetPreferencesTypeUser("Orasianu");
            TypesGridView.ItemsSource = preferences.Where(c => c.Category == "Type").Select(d => d.Name).ToArray();
                foreach (string i in preferencesUser)
            {
                if (TypesGridView.Items != null)
                    foreach ( var j in TypesGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            TypesGridView.SelectedItems.Add(j);
                        }
                    }
            }

            IsOperationInProgress = false;
        }

        private async void FillGroups()
        {
            IsOperationInProgress = true;
            _groupManager = new GroupManager(_dbWrapper);
            var groups = await Task.Run(() => _groupManager.GetAllGroups());
            List<string> groupsUser = _groupManager.GetUserGroup("Orasianu");
            GroupsGridView.ItemsSource = groups.Select(c => c.Trim()).ToArray();
            foreach (string i in groupsUser)
            {
                if (GroupsGridView.Items != null)
                    foreach (var j in GroupsGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            GroupsGridView.SelectedItems.Add(j);
                        }
                    }
            }
            IsOperationInProgress = false;
        }


     

        private void Initialize()
        {
           
            FillText();
            FillPassword();
            FillConfirmPassword();

        }
        public byte[] Picture { get; set; }

        public ImageSource Photo
        {
            get
            {
                if (Picture == null)
                    return null;
                byte[] imageBytes = Picture;

                var image = new BitmapImage();
                var ms = new InMemoryRandomAccessStream();
                ms.AsStreamForWrite().Write(imageBytes, 0, imageBytes.Length);
                ms.Seek(0);

                image.SetSource(ms);
                ImageSource src = image;

                return src;

            }
        }

        public void FillImg()
        {
            Picture = _userManager.GetImage("Orasianu");

        }

        public static string UserName { set; get; }
       
        public void FillText()
        {
           text_box_email.Text = _userManager.GetMail("Orasianu");
        }
        public void FillPassword()
        {
             password_box.Password = "Password";
        }
        public void FillConfirmPassword()
        {
            confirm_password_box.Password = "Password";
        }

        private void OrderHistory(object sender, RoutedEventArgs e)//OrderHistory
        {
            var frame = (Frame) Window.Current.Content;
            var page = (MainPage) frame.Content;
            VisualStateManager.GoToState(page, "CancelOrderControl", true);
        }
        private  async void Browse(object sender, RoutedEventArgs e)
        {
       
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            open.FileTypeFilter.Clear();
            open.FileTypeFilter.Add(".bmp");
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".jpg");

            StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                   /* BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelHeight = 250;
                    bitmapImage.DecodePixelWidth = 250;

                    await bitmapImage.SetSourceAsync(fileStream);
                    _image = bitmapImage;*/

                    var reader = new Windows.Storage.Streams.DataReader(fileStream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)fileStream.Size);

                    byte[] pixels = new byte[fileStream.Size];

                    reader.ReadBytes(pixels);

                    _image = pixels;
                }
            }
        }
        
        private void LogOut(object sender, RoutedEventArgs e)//LogOut
        {
            //this.Frame.Navigate(typeof(MainPage), null);
        }

        //public bool Checked { get; set; }

        private void FillRadioButton()
        {
            var n = _userManager.GetNewsletter(UserName);

            if (n == 1)
            {
                radio_button.IsChecked = true;
            }
            else
            {
                radio_button.IsChecked = false;
            }
        }
        
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            bool var;

            if (radio_button.IsChecked == true)

                var = true;
            else
                var = false;

            var countriesPreferences = CountriesGridView.SelectedItems.Select(c => c.ToString());
            var typesPreferences = TypesGridView.SelectedItems.Select(c => c.ToString());
            var preferences = countriesPreferences.Concat(typesPreferences).ToList();
            var groups = GroupsGridView.SelectedItems.Select(c => c.ToString()).ToList();
            _userManager.UpdateUser(
                "Orasianu",
                var,
                text_box_email.Text,
                password_box.Password,
                confirm_password_box.Password,
                preferences,
                groups
                );

        }
    }
}
