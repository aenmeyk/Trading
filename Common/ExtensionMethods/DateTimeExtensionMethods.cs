using System;

namespace Common.ExtensionMethods
{
    public static class DateTimeExtensionMethods
    {
        public static string ToSqlString(this DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }
    }
}
