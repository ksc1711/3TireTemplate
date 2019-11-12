using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WCMS.FrameWork.Data.Configuration
{
    public static class DBConfigSettings
    {
        /// <summary>
        /// 1. GlobalFramework.config Sample
        /// <?xml version="1.0"?>
        /// <connectionStrings>
        ///   <add key="Test" value="Data Source=172.21.1.203,2433;Initial Catalog=COMMON;User ID=devteam;Password=DusqhdDlstkd200%;"/>
        ///   <add key="Test_ProviderName" value="System.Data.SqlClient"/>
        /// </connectionStrings>
        /// 2. 기본 Provider : System.Data.SqlClient
        /// </summary>
        const string GlobalConfigPath = @"D:\GlobalFramework.config";

        /// <summary>
        /// ConnectionString Custom AppSettings
        /// </summary>
        public static DBConfigReader ConnectionStrings
        {
            get
            {
                return CustomConfigSettings.GetSettings<DBConfigReader>(GlobalConfigPath, "connectionStrings");
            }
        }

        public class DBConfigReader : XmlConfigReader
        {
            public DBConfigReader() { }
            public DBConfigReader(string configFile) : base(configFile) { }
            public DBConfigReader(string configFile, string settingName) : base(configFile, settingName) { }

            /// <summary>ConnectionString 반환</summary>
            public string GetConnectionString(string key)
            {
                return GetValue<string>(key, "");
            }

            /// <summary>
            /// ProviderName 반환
            /// Default : System.Data.SqlClient
            /// </summary>
            public string GetProviderName(string key)
            {
                return GetValue<string>(key + "_ProviderName", "System.Data.SqlClient");
            }
        }
    }
}
