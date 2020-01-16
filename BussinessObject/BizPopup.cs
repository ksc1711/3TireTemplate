using System.Collections.Generic;
using System.Configuration;
using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizPopup
    {
        public int SetPopupData(PopupData popupData, string memberId)
        {
            return new DacPopup(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).SetPopupData(popupData, memberId);
        }
    }
}
