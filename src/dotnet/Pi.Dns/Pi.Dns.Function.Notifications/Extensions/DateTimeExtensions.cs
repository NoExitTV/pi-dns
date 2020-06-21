using System;

namespace Pi.Dns.Function.Notifications.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetHoursOfIncrements(this DateTime dateTime)
        {
            if (dateTime.Month == 1 && dateTime.Day == 1)
                return 8760; // 24 * 365 (1 year)
            if (dateTime.Day == 1)
                return 720; // 24 * 30 (30d)
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                return 168; // 24 * 7 (7d)
            else
                return 24;
        }
    }
}
