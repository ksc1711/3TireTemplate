using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
