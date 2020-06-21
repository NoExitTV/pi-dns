namespace Pi.Dns.Function.Notifications.Extensions
{
    public static class IntExtensions
    {
        public static string PrintableTimeSpan(this int hours)
        {
            // Exactly one year
            if (hours == 8760)
                return "year";

            // Exactly one week
            if (hours == 168)
                return "week";

            // Larger than one day
            if (hours > 24)
            {
                if (hours % 24 == 0)
                {
                    return $"{hours / 24} days";
                }
                else
                {
                    return $"{hours / 24} day(s) and {hours % 24} hour(s)";
                }
            }

            if (hours > 0)
                return $"{hours}h";

            return $"UNKNOWN-{hours}";
        }
    }
}
