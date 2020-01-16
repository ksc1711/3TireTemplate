using System.Collections.Generic;
using System.Configuration;
using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizImage
    {
        public string SetImageData(ImageData imgeData, string memberId )
        {
            return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).SetImageData(imgeData, memberId);
        }

        public List<ImageData> GetImageList(string imageKeyword)
        {
            return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetImageList(imageKeyword);
        }

        public ImageData GetImageData(int idx)
        {
            return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetImageData(idx);
        }

        public int UpdateImageData(ImageData imgeData, string memberId)
        {
            return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).UpdateImageData(imgeData, memberId);
        }

        public int DeleteImageData(int idx, string memberId)
        {
            return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).DeleteImageData(idx, memberId);
        }
        
    }
}
