using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Domain.Helper
{
    /// <summary>
    /// 类型帮助类
    /// </summary>
    public class TypeHelper
    {
        #region 转Int

        /// <summary>
        /// 将string类型转换成int类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int StringToInt(string s, int defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                int result;
                if (int.TryParse(s, out result))
                    return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string类型转换成int类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <returns></returns>
        public static int StringToInt(string s)
        {
            return StringToInt(s, 0);
        }

        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ObjectToInt(object o, int defaultValue)
        {
            if (o != null)
                return StringToInt(o.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <returns></returns>
        public static int ObjectToInt(object o)
        {
            return ObjectToInt(o, 0);
        }

        #endregion

        #region 转Bool

        /// <summary>
        /// 将string类型转换成bool类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool StringToBool(string s, bool defaultValue)
        {
            if (s == "false")
                return false;
            else if (s == "true")
                return true;

            return defaultValue;
        }

        /// <summary>
        /// 将string类型转换成bool类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <returns></returns>
        public static bool ToBool(string s)
        {
            return StringToBool(s, false);
        }

        /// <summary>
        /// 将object类型转换成bool类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ObjectToBool(object o, bool defaultValue)
        {
            if (o != null)
                return StringToBool(o.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成bool类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <returns></returns>
        public static bool ObjectToBool(object o)
        {
            return ObjectToBool(o, false);
        }

        #endregion

        #region 转DateTime

        /// <summary>
        /// 将string类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string s, DateTime defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                DateTime result;
                if (DateTime.TryParse(s, out result))
                    return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// 将string类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string s)
        {
            return StringToDateTime(s, DateTime.Now);
        }

        /// <summary>
        /// 将object类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime ObjectToDateTime(object o, DateTime defaultValue)
        {
            if (o != null)
                return StringToDateTime(o.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <returns></returns>
        public static DateTime ObjectToDateTime(object o)
        {
            return ObjectToDateTime(o, DateTime.Now);
        }


        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        #endregion

        #region 转Decimal

        /// <summary>
        /// 将string类型转换成decimal类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal StringToDecimal(string s, decimal defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                decimal result;
                if (decimal.TryParse(s, out result))
                    return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string类型转换成decimal类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <returns></returns>
        public static decimal StringToDecimal(string s)
        {
            return StringToDecimal(s, 0m);
        }

        /// <summary>
        /// 将object类型转换成decimal类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ObjectToDecimal(object o, decimal defaultValue)
        {
            if (o != null)
                return StringToDecimal(o.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成decimal类型
        /// </summary>
        /// <param name="s">目标对象</param>
        /// <returns></returns>
        public static decimal ObjectToDecimal(object o)
        {
            return ObjectToDecimal(o, 0m);
        }

        #endregion

        #region Datetime转string

        /// <summary>
        /// 将datetime类型转换string成类型
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string DateTimeToString(Nullable<DateTime> s)
        {
            if (s == null)
                return string.Empty;
            else
                return ((DateTime)s).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 将datetime类型转换string成类型
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string DateToString(Nullable<DateTime> s)
        {
            if (s == null)
                return string.Empty;
            else
                return ((DateTime)s).ToString("yyyy-MM-dd");
        }
        #endregion

        /// <summary>
        /// 将string类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime DateTimeFormat(DateTime d)
        {
            if (d.ToString().Contains("0001-01-01") || d.ToString().Contains("0001/1/1"))
            {
                d = DateTime.Now;
            }
            return Convert.ToDateTime(d.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo));

            //return DateTime.ParseExact(d.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// string转System.Guid
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns></returns>
        public static System.Guid StringToGuid(string s)
        {
            System.Guid guid = System.Guid.Empty;
            System.Guid.TryParse(s, out guid);
            return guid;
        }

        /// <summary>
        /// Nullable转System.Guid
        /// </summary>
        /// <param name="ng"></param>
        /// <returns></returns>
        public static System.Guid NullableToGuid(Nullable<Guid> ng)
        {
            return ng == null ? Guid.Empty : ng.Value;
        }

    }
}
