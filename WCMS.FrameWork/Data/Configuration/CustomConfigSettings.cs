using System;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Reflection;

namespace WCMS.FrameWork.Data.Configuration
{
    /// <summary>
    /// 사용자 AppSettings 설정파일을 XmlConfigReader를 통해 접근할 수 있게 해주는 클래스
    /// </summary>
    public static class CustomConfigSettings
    {
        /// <summary>
        /// 지정된 설정파일에서 appSettings 부분을 읽는다.
        /// </summary>
        /// <param name="configFile">config 파일 경로</param>
        /// <returns></returns>
        public static XmlConfigReader GetSettings(string configFile)
        {
            return GetSettings(configFile, "appSettings");
        }
        
        /// <summary>
        /// 지정된 설정파일에서 지정된 setting 부분을 읽는다.
        /// </summary>
        /// <param name="configFile">config 파일 경로</param>
        /// <param name="settingName">요소 명</param>
        /// <returns></returns>
        public static XmlConfigReader GetSettings(string configFile, string settingName)
        {
            if (HttpContext.Current == null)
            {
                return new XmlConfigReader(configFile, settingName);
            }
            if (HttpContext.Current.Cache[settingName] == null)
            {
                HttpContext.Current.Cache.Insert(settingName, new XmlConfigReader(configFile, settingName), new CacheDependency(configFile));
            }

            return (XmlConfigReader)HttpContext.Current.Cache[settingName];
        }
        
        /// <summary>
        /// 지정된 설정파일에서 appSettings 부분을 읽는다.
        /// </summary>
        /// <typeparam name="T">XmlConfigReader 상속 클래스</typeparam>
        /// <param name="configFile">config 파일 경로</param>
        /// <returns></returns>
        public static T GetSettings<T>(string configFile) where T : XmlConfigReader
        {
            return GetSettings<T>(configFile, "appSettings");
        }

        /// <summary>
        /// 지정된 설정파일에서 지정된 setting 부분을 읽는다.
        /// </summary>
        /// <typeparam name="T">XmlConfigReader 상속 클래스</typeparam>
        /// <param name="configFile">config 파일 경로</param>
        /// <param name="settingName">요소 명</param>
        /// <returns></returns>
        public static T GetSettings<T>(string configFile, string settingName) where T : XmlConfigReader
        {
            if (HttpContext.Current == null)
            {
                T obj = CreateCustomConfigReader<T>(configFile, settingName);
                return obj;
            }
            if (HttpContext.Current.Cache[settingName] == null)
            {
                T obj = CreateCustomConfigReader<T>(configFile, settingName);
                HttpContext.Current.Cache.Insert(settingName, obj, new CacheDependency(configFile));
            }

            return (T)HttpContext.Current.Cache[settingName];
        }

        private static T CreateCustomConfigReader<T>(string configFile, string settingName) where T : XmlConfigReader
        {
            ConstructorInfo constructorInfoObj = typeof(T).GetConstructor(new Type[] { typeof(string), typeof(string) });
            T obj = constructorInfoObj.Invoke(new object[] { configFile, settingName }) as T;
            return obj;
        }
    }
}
