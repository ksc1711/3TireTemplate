using System.Collections.Generic;
using System.Configuration;
using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizCommon
    {
        public CommonCodeData GetCommonData(string commonCode)
        {
            return null;
            //return new DacImage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetCommonData(commonCode);
        }
    }
}
