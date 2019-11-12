using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCMS.Web
{
	public static class StringExtensions
	{
		/// <summary>
		/// Removes dashes ("-") from the given object value represented as a string and returns an emtpy string("")
		/// When the instance type could not be represented as a string.
		/// <para>
		///		Note: This will return the type name of given instance if the runtime type of the given instance is not a string!
		/// </para>
		/// </summary>
		/// <param name="value">The object instance to undash when represented as its string value.</param>
		/// <returns></returns>
		public static String UnDash(this object value)
		{
			return ((value as String) ?? String.Empty).UnDash();
		}

		/// <summary>
		/// Removes dashes ("-") from the given string value.
		/// </summary>
		/// <param name="value">The string value that optionally contains dashes.</param>
		/// <returns></returns>
		public static String UnDash(this String value)
		{
			return (value ?? String.Empty).Replace("-", String.Empty);
		}

        public static DateTime ToDateTime(this object value)
        {
            return Convert.ToDateTime(value);
        }

        public static String ToIdDateType(this DateTime value)
        {
            try
            {
                return value > DateTime.MinValue ? value.ToString("dd.MM.yyyy") : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// if the value is true, return 'Y'. else return 'N'
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static char ToChar(this bool b)
        {
            return b ? 'Y' : 'N';
        }

        public static string ToSexName(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "Unknown";
            }
            return value.Equals("M", StringComparison.InvariantCultureIgnoreCase) ? "Male" : "FeMale";
        }

        public static String ToIdLongDateType(this DateTime value)
        {
            try
            {
                return value > DateTime.MinValue ? value.ToString("dd.MM.yyyy HH:mm") : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static String ToIdMoneyType(this Decimal value)
        {
            return string.Format("{0:##,##0.00}",value); //1,234,567 값이 0일경우 0을 찍음 
        }

        public static String ToIdMoneyType(this float value)
        {
            return string.Format("{0:##,##0.00}", value); //1,234,567 값이 0일경우 0을 찍음 
        }

        public static String ToIdMoneyType(this Int32 value)
        {
            return string.Format("{0:##,##0}", value); //1,234,567 값이 0일경우 0을 찍음 
        }

        public static String ToDbDateTime(this string value)
        {
            string returnValue = string.Empty;
            try
            {
                var splitDate = value.Split('.');
                returnValue = string.Format("{0}-{1}-{2}", splitDate[2], splitDate[1], splitDate[0]);
            }
            catch
            {
                returnValue = string.Empty;
            }

            return returnValue;
        }

        public static Int32 ToInt32(this string value)
        {
            int returnValue = 0;

            var returnFlag = Int32.TryParse(value, out returnValue);

            return returnFlag ? returnValue : 0; //1,234,567 값이 0일경우 0을 찍음 
        }

        public static string ToSeoFriendlyString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.ToLower();
            str = str.Replace(",", "");
            str = str.Replace("&", "and");
            str = str.Replace(" ", "-");
            str = str.Replace(".", "");
            return str;
        }
	}
}