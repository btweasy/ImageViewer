using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageViewerApp.Models;

namespace ImageViewerApp.UserControls
{
    public partial class ImageThumbnail : UserControl
    {
        public ImageThumbnail()
        {
            InitializeComponent();
            this.MouseLeftButtonUp += ImageThumbnail_MouseLeftButtonUp;
        }

        private void StackPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (DataContext is ImageItem item)
            {
                var menu = MetadataContextMenu;
                menu.Items.Clear();

                void AddCopyableItem(string label, string value)
                {
                    var menuItem = new MenuItem { Header = $"{label}: {value}" };
                    menuItem.Click += (s, args) =>
                    {
                        Clipboard.SetText(value);
                        ShowCopiedPopup();
                    };
                    menu.Items.Add(menuItem);
                }

                AddCopyableItem("Name", item.FileName);
                AddCopyableItem("Path", item.FilePath);
                menu.Items.Add(new Separator());
                AddCopyableItem("Format", item.Format);
                AddCopyableItem("Size", $"{item.FileSizeKb} KB");
                AddCopyableItem("Resolution", item.Resolution);
                AddCopyableItem("Modified", item.DateModified.ToString());
            }
        }

        private void ImageThumbnail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ImageItem item)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowFullImage(item.FilePath);
            }
        }

        private async void ShowCopiedPopup()
        {
            CopyPopup.IsOpen = true;
            await Task.Delay(1500); 
            CopyPopup.IsOpen = false;
        }
    }
}
