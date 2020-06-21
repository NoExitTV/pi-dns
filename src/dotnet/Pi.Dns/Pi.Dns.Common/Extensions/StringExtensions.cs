namespace Pi.Dns.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return string as int, default to 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int AsInt(this string value)
        {
            if (int.TryParse(value, out var result))
                return result;

            return 0;
        }

        /// <summary>
        /// Return string as double, default to 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double AsDouble(this string value)
        {
            if (double.TryParse(value, out var result))
                return result;

            return 0;
        }
    }
}
