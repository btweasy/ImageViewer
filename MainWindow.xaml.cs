using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageViewerApp.Models;
using WinForms = System.Windows.Forms;

namespace ImageViewerApp
{
    public partial class MainWindow : Window
    {
        private List<string>? currentImagePaths;
        private int currentImageIndex;

        public ObservableCollection<ImageItem> Images { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            ImageList.ItemsSource = Images;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new WinForms.FolderBrowserDialog();
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                LoadImagesFromFolder(dialog.SelectedPath);
            }
        }

        public void ShowFullImage(string path)
        {
            currentImagePaths = ImageList.Items.Cast<ImageItem>()
                .Select(i => i.FilePath)
                .Where(p => p != null)
                .ToList()!;

            currentImageIndex = currentImagePaths.IndexOf(path);

            FullImage.Source = new BitmapImage(new Uri(path));
            Overlay.Visibility = Visibility.Visible;
        }

        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentImagePaths == null || currentImagePaths.Count == 0) return;

            currentImageIndex = (currentImageIndex - 1 + currentImagePaths.Count) % currentImagePaths.Count;
            ShowFullImage(currentImagePaths[currentImageIndex]);
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentImagePaths == null || currentImagePaths.Count == 0) return;

            currentImageIndex = (currentImageIndex + 1) % currentImagePaths.Count;
            ShowFullImage(currentImagePaths[currentImageIndex]);
        }

        private void LoadImagesFromFolder(string folderPath)
        {
            Images.Clear();

            var imageFiles = Directory.GetFiles(folderPath, "*.*")
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase));

            foreach (var path in imageFiles)
            {
                var thumb = new BitmapImage();
                thumb.BeginInit();
                thumb.UriSource = new Uri(path);
                thumb.DecodePixelWidth = 100;
                thumb.CacheOption = BitmapCacheOption.OnLoad;
                thumb.EndInit();
                thumb.Freeze();

                var fileInfo = new FileInfo(path);

                string resolution;
                try
                {
                    using var img = System.Drawing.Image.FromFile(path);
                    resolution = $"{img.Width}x{img.Height}";
                }
                catch
                {
                    resolution = "Unknown";
                }

                Images.Add(new ImageItem
                {
                    FilePath = path,
                    Thumbnail = thumb,
                    FileSizeKb = fileInfo.Length / 1024,
                    DateModified = fileInfo.LastWriteTime,
                    Format = fileInfo.Extension?.TrimStart('.').ToUpper(),
                    Resolution = resolution
                });
            }
        }
    }
}
