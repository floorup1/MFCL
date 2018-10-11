using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MFCL.Helpers;
using MFCL.Models;
using MFCL.Services;

using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MFCL.Views
{
    public sealed partial class ImageGalleryDetailPage : Page, INotifyPropertyChanged
    {
        private DispatcherTimer _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
        private object _selectedImage;
        private ObservableCollection<Serv> _source;

        public object SelectedImage
        {
            get => _selectedImage;
            set
            {
                Set(ref _selectedImage, value);
                ApplicationData.Current.LocalSettings.SaveString(ImageGalleryPage.ImageGallerySelectedIdKey, ((Serv)SelectedImage).ID);
            }
        }

        public ObservableCollection<Serv> Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public ImageGalleryDetailPage()
        {
            // TODO WTS: Replace this with your actual data
            Source = SampleDataService.GetGalleryServ();
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var sampleImageId = e.Parameter as string;
            if (!string.IsNullOrEmpty(sampleImageId) && e.NavigationMode == NavigationMode.New)
            {
                SelectedImage = Source.FirstOrDefault(i => i.ID == sampleImageId);
            }
            else
            {
                var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGalleryPage.ImageGallerySelectedIdKey);
                if (!string.IsNullOrEmpty(selectedImageId))
                {
                    SelectedImage = Source.FirstOrDefault(i => i.ID == selectedImageId);
                }
            }

            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGalleryPage.ImageGalleryAnimationOpen);
            animation?.TryStart(previewImage);
            showFlipView.Begin();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                previewImage.Visibility = Visibility.Visible;
                ConnectedAnimationService.GetForCurrentView()?.PrepareToAnimate(ImageGalleryPage.ImageGalleryAnimationClose, previewImage);
            }
        }

        private void OnShowFlipViewCompleted(object sender, object e) => flipView.Focus(FocusState.Programmatic);

        private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
