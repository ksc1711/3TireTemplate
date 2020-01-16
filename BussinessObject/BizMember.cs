using System.Collections.Generic;
using System.Configuration;
using WCMS.DAC;
using WCMS.Data;

namespace WCMS.Bussiness
{
    public class BizMember
    {
        public MemberData GetLoginData(string memberId, string memberPw)
        {
            return new DacMember(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetLoginData(memberId, memberPw);
        }

        public string SetSignUp(string memberId, string memberPw, string memberName, string memberPhone)
        {
            return new DacMember(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).SetSignUp(memberId, memberPw, memberName, memberPhone);
        }

        public List<MemberData> GetLoginList(string depatment)
        {
            return new DacMember(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetLoginList(depatment);
        }
    }
}
