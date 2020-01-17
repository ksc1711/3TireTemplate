using System.Collections.Generic;
using System.Configuration;
using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizCommon
    {
        public List<CommonCodeData> GetCommonCodeData(string commonCode)
        {
            return new DacCommon(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetCommonCodeData(commonCode);
        }
    }
}
