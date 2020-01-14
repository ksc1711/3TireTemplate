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
    }
}
