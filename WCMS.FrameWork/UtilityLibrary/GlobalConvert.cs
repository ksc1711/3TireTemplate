using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCMS.FrameWork.UtilityLibrary
{
    public class GlobalConvert
    {
        /// <summary>
        /// 기본값 반환 (값, 참조타입 지원)
        /// </summary>
        public static T DefaultValue<T>()
        {
            return (T)((typeof(T).IsValueType) ? Activator.CreateInstance(typeof(T)) : default(T));
        }

        /// <summary>
        /// 형 변환 (기본값 처리)
        /// </summary>
        public static T ChangeType<T>(object val) where T : IConvertible
        {
            T defaultValue = DefaultValue<T>();
            return ChangeType<T>(val, defaultValue);
        }

        /// <summary>
        /// 형 변환 (기본값 처리)
        /// </summary>
        public static T ChangeType<T>(object val, T defaultValue) where T : IConvertible
        {
            try
            {
                T typedval = (T)Convert.ChangeType(val, typeof(T));
                return (typedval != null) ? typedval : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
