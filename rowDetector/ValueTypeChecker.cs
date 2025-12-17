using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace rowDetector
{
    public static class ValueTypeChecker
    {
        public static bool IsValid(string text, ColumnValueType type)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            switch (type)
            {
                case ColumnValueType.Integer:
                    return int.TryParse(Clean(text), out _);

                case ColumnValueType.Decimal:
                    return decimal.TryParse(
                        Clean(text),
                        NumberStyles.Any,
                        CultureInfo.GetCultureInfo("tr-TR"),
                        out _);

                case ColumnValueType.Percentage:
                    return int.TryParse(Clean(text), out var p) && p >= 0 && p <= 100;

                case ColumnValueType.Date:
                    return DateTime.TryParse(text, out _);

                case ColumnValueType.String:
                    return text.Length > 2;

                default:
                    return false;
            }
        }

        private static string Clean(string t)
            => t.Replace("%", "").Replace(" ", "");
    }
}
