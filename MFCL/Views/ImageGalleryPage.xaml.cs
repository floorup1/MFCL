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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MFCL.Views
{
    public sealed partial class ImageGalleryPage : Page, INotifyPropertyChanged
    {
        public const string ImageGallerySelectedIdKey = "ImageGallerySelectedIdKey";
        public const string ImageGalleryAnimationOpen = "ImageGallery_AnimationOpen";
        public const string ImageGalleryAnimationClose = "ImageGallery_AnimationClose";

        private ObservableCollection<Serv> _source;

        public ObservableCollection<Serv> Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public ImageGalleryPage()
        {
            Loaded += ImageGalleryPage_Loaded;

            // TODO WTS: Replace this with your actual data
            Source = SampleDataService.GetGalleryServ();
            InitializeComponent();
        }

        private async void ImageGalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGallerySelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGalleryAnimationClose);
                if (animation != null)
                {
                    var item = ImagesGridView.Items.FirstOrDefault(i => ((Serv)i).ID == selectedImageId);
                    ImagesGridView.ScrollIntoView(item);
                    await ImagesGridView.TryStartConnectedAnimationAsync(animation, item, "galleryImage");
                }

                ApplicationData.Current.LocalSettings.SaveString(ImageGallerySelectedIdKey, string.Empty);
            }
        }

        private void ImagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as Serv;
            ImagesGridView.PrepareConnectedAnimation(ImageGalleryAnimationOpen, selected, "galleryImage");
            NavigationService.Navigate<ImageGalleryDetailPage>(selected.ID);
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
