﻿using Files.Filesystem;
using Files.Interacts;
using GalaSoft.MvvmLight;
using System;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Files
{

    public sealed partial class Properties : Page
    {
        public AppWindow propWindow;
        public ItemPropertiesViewModel ItemProperties { get; } = new ItemPropertiesViewModel();
        public Properties()
        {
            this.InitializeComponent();
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                Loaded += Properties_Loaded;
            }
            else
            {
                this.OKButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void Properties_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Collect AppWindow-specific info
            propWindow = Interaction.AppWindows[this.UIContext];
            if (App.CurrentInstance.ContentPage.IsItemSelected)
            {
                var selectedItem = App.CurrentInstance.ContentPage.SelectedItem;
                IStorageItem selectedStorageItem;
                try 
                {
                    selectedStorageItem = await StorageFolder.GetFolderFromPathAsync(selectedItem.ItemPath);
                }
                catch (Exception)
                {
                    // Not a folder, so attempt to get as StorageFile
                    selectedStorageItem = await StorageFile.GetFileFromPathAsync(selectedItem.ItemPath);
                }
                ItemProperties.ItemName = selectedItem.ItemName;
                ItemProperties.ItemType = selectedItem.ItemType;
                ItemProperties.ItemPath = selectedItem.ItemPath;
                ItemProperties.ItemSize = selectedItem.FileSize;
                ItemProperties.LoadFileIcon = selectedItem.LoadFileIcon;
                ItemProperties.LoadFolderGlyph = selectedItem.LoadFolderGlyph;
                ItemProperties.LoadUnknownTypeGlyph = selectedItem.LoadUnknownTypeGlyph;
                ItemProperties.ItemModifiedTimestamp = selectedItem.ItemDateModified;
                ItemProperties.ItemCreatedTimestamp = ListedItem.GetFriendlyDate(selectedStorageItem.DateCreated);

                if (!App.CurrentInstance.ContentPage.SelectedItem.LoadFolderGlyph)
                {
                    var thumbnail = await (await StorageFile.GetFileFromPathAsync(App.CurrentInstance.ContentPage.SelectedItem.ItemPath)).GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 40, Windows.Storage.FileProperties.ThumbnailOptions.ResizeThumbnail);
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    ItemProperties.FileIconSource = bitmap;
                }
            }
            else
            {
                var parentDirectory = App.CurrentInstance.ViewModel.CurrentFolder;
                var parentDirectoryStorageItem = await StorageFolder.GetFolderFromPathAsync(parentDirectory.ItemPath);
                ItemProperties.ItemName = parentDirectory.ItemName;
                ItemProperties.ItemType = parentDirectory.ItemType;
                ItemProperties.ItemPath = parentDirectory.ItemPath;
                ItemProperties.ItemSize = parentDirectory.FileSize;
                ItemProperties.LoadFileIcon = false;
                ItemProperties.LoadFolderGlyph = true;
                ItemProperties.LoadUnknownTypeGlyph = false;
                ItemProperties.ItemModifiedTimestamp = parentDirectory.ItemDateModified;
                ItemProperties.ItemCreatedTimestamp = ListedItem.GetFriendlyDate(parentDirectoryStorageItem.DateCreated);
            }
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                await propWindow.CloseAsync();
            }
        }
    }

    public class ItemPropertiesViewModel : ViewModelBase
    {
        private string _ItemName;
        private string _ItemType;
        private string _ItemPath;
        private string _ItemSize;
        private string _ItemCreatedTimestamp;
        private string _ItemModifiedTimestamp;
        private ImageSource _FileIconSource;
        private bool _LoadFolderGlyph;
        private bool _LoadUnknownTypeGlyph;
        private bool _LoadFileIcon;

        public string ItemName
        {
            get => _ItemName;
            set => Set(ref _ItemName, value);
        }
        public string ItemType
        {
            get => _ItemType;
            set => Set(ref _ItemType, value);
        }
        public string ItemPath
        {
            get => _ItemPath;
            set => Set(ref _ItemPath, value);
        }
        public string ItemSize
        {
            get => _ItemSize;
            set => Set(ref _ItemSize, value);
        }
        public string ItemCreatedTimestamp
        {
            get => _ItemCreatedTimestamp;
            set => Set(ref _ItemCreatedTimestamp, value);
        }
        public string ItemModifiedTimestamp
        {
            get => _ItemModifiedTimestamp;
            set => Set(ref _ItemModifiedTimestamp, value);
        }
        public ImageSource FileIconSource
        {
            get => _FileIconSource;
            set => Set(ref _FileIconSource, value);
        }
        public bool LoadFolderGlyph
        {
            get => _LoadFolderGlyph;
            set => Set(ref _LoadFolderGlyph, value);
        }
        public bool LoadUnknownTypeGlyph
        {
            get => _LoadUnknownTypeGlyph;
            set => Set(ref _LoadUnknownTypeGlyph, value);
        }
        public bool LoadFileIcon
        {
            get => _LoadFileIcon;
            set => Set(ref _LoadFileIcon, value);
        }
    }
}
