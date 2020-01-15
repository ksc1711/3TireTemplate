using System;
using System.ComponentModel;

namespace WCMS.Data
{
    public class ImageData
    {
        public int idx { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string imageSize { get; set; }
        public string imageKeyword { get; set; }
        public string useYn { get; set; }
        public DateTime regDate { get; set; }
        public string regUser { get; set; }
        
        public DateTime modDate { get; set; }
        public string modUser { get; set; }
        public int totalCount { get; set; }
        public int pageCount { get; set; }

    }
}
