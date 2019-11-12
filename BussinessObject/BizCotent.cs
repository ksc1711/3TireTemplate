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
        public List<Content> GetContentList(Content pContent)
        {
            return new DacContent(ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString).GetContentList(pContent);
        }
    }
}
