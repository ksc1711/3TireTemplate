using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCMS.Data
{
    public class PopupData
    {
        public int idx { get; set; }
        public string title { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string joinMember { get; set; }
        public string popupType { get; set; }
        public string descript { get; set; }
        public string popupHtml { get; set; }
        public string popupScript { get; set; }
        public string useYn { get; set; }
        public DateTime regDate { get; set; }
        public string regUser { get; set; }

        public DateTime modDate { get; set; }
        public string modUser { get; set; }

    }
}
