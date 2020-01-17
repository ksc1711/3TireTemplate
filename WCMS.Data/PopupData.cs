using System;
using System.ComponentModel;

namespace WCMS.Data
{
    public class PopupData
    {
        [DefaultValue(0)]
        public int idx { get; set; }
        [DefaultValue("")]
        public string title{ get; set; }
        [DefaultValue("")]
        public string startDate { get; set; }
        [DefaultValue("")]
        public string endDate { get; set; }
        [DefaultValue("")]
        public string joinMember { get; set; }
        [DefaultValue("")]
        public string popupType { get; set; }
        public string descript { get; set; }
        public string popupHtml { get; set; }
        public string popupScript { get; set; }
        public string useYn { get; set; }
        public DateTime regDate { get; set; }
        [DefaultValue("")]
        public string regUser { get; set; }

        public DateTime modDate { get; set; }
        public string modUser { get; set; }

    }
}
