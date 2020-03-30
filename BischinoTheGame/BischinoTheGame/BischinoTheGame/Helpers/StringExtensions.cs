using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rooms.Helpers
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string FirstUpperCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (char.IsUpper(value[0]))
                return value;

            if (value.Length == 1)
                return value.ToUpper();

            return $"{char.ToString(value.First()).ToUpper()}{value.Substring(1)}";
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

    }
}
