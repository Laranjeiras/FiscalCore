using System;

namespace FiscalCore.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToUtcString(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }
    }
}