using System;
using System.Globalization;

namespace CslAppSystem
{
    internal static class Extensions
    {
        internal static DateTime ParseDate(this string value)
        {
            DateTime
                .TryParseExact
                (
                    value,
                    "dd/MM/yyyy",
                    new CultureInfo("pt-BR"),
                    DateTimeStyles.None,
                    out DateTime dateTimeValue
                );
            return dateTimeValue;
        }

        internal static bool IsDate(this string value)
        {
            return DateTime
                .TryParseExact
                (
                    value,
                    "dd/MM/yyyy",
                    new CultureInfo("pt-BR"),
                    DateTimeStyles.None,
                    out _
                );
        }

        internal static bool IsNotDate(this string value)
        {
            return value.IsDate() == false;
        }

        internal static bool IsLong(this string value)
        {
            return long.TryParse(value, out _);
        }
        internal static long ParseLong(this string value)
        {
            if (long.TryParse(value, out long id)) return id;
            return 0;
        }
    }
}
