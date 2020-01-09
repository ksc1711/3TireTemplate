using System.Collections.Generic;
using System.Configuration;

using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizCotent
    {
        public List<ContentData> GetContentList(ContentData pContent)
        {
            return new DacContent(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetContentList(pContent);
        }
    }
}
