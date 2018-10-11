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
    public sealed partial class ImageGallery1Page : Page, INotifyPropertyChanged
    {
        public const string ImageGallery1SelectedIdKey = "ImageGallery1SelectedIdKey";
        public const string ImageGallery1AnimationOpen = "ImageGallery1_AnimationOpen";
        public const string ImageGallery1AnimationClose = "ImageGallery1_AnimationClose";

        private ObservableCollection<SampleImage> _source;

        public ObservableCollection<SampleImage> Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public ImageGallery1Page()
        {
            Loaded += ImageGallery1Page_Loaded;

            // TODO WTS: Replace this with your actual data
            Source = SampleDataService.GetGallerySampleData();
            InitializeComponent();
        }

        private async void ImageGallery1Page_Loaded(object sender, RoutedEventArgs e)
        {
            var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGallery1SelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGallery1AnimationClose);
                if (animation != null)
                {
                    var item = ImagesGridView.Items.FirstOrDefault(i => ((SampleImage)i).ID == selectedImageId);
                    ImagesGridView.ScrollIntoView(item);
                    await ImagesGridView.TryStartConnectedAnimationAsync(animation, item, "galleryImage");
                }

                ApplicationData.Current.LocalSettings.SaveString(ImageGallery1SelectedIdKey, string.Empty);
            }
        }

        private void ImagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as SampleImage;
            ImagesGridView.PrepareConnectedAnimation(ImageGallery1AnimationOpen, selected, "galleryImage");
            NavigationService.Navigate<ImageGallery1DetailPage>(selected.ID);
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
