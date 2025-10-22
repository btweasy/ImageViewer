
using System;
using System.Windows.Media.Imaging;

namespace ImageViewerApp.Models
{
    public class ImageItem
    {
        public string ?FilePath { get; set; }
        public string FileName => System.IO.Path.GetFileName(FilePath);
        public BitmapImage Thumbnail { get; set; }
        public string FullImagePath => FilePath;

        // Meta Dane
        public string ?Resolution { get; set; }        
        public long ?FileSizeKb { get; set; }          
        public DateTime ?DateModified { get; set; }    
        public string ?Format { get; set; }

    }
}
