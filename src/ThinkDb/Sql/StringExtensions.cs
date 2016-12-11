using System;
using System.Globalization;
using System.Text;

namespace ThinkDb.Sql
{
    internal static class StringExtensions
    {
        public static string Enquote(this string text, char startQuote, char endQuote)
        {
            if (text.Length > 0 && text[0] != startQuote)
                text = startQuote + text;
            if (text.Length > 0 && text[text.Length - 1] != endQuote)
                text = text + endQuote;
            return text;
        }

        public static string Enquote(this string text, char quote)
        {
            return Enquote(text, quote, quote);
        }

        /// <summary>
        /// Returns true is the provided string is a valid .NET symbol
        /// </summary>
        public static bool IsIdentifier(this string name)
        {
            for (int index = 0; index < name.Length; index++) {
                var category = char.GetUnicodeCategory(name, index);
                // this is not nice, but I found no other way to identity a valid identifier
                switch (category) {
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.LetterNumber:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.ConnectorPunctuation:
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        public static bool ContainsCase(this string text, string find, bool ignoreCase)
        {
            if (text == null)
                return false;
            var comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            var endIndex = text.IndexOf(find, 0, comparison);
            return endIndex >= 0;
        }

        public static string ReplaceCase(this string text, string find, string replace, bool ignoreCase)
        {
            var result = new StringBuilder();
            var comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            for (int index = 0; ; ) {
                var endIndex = text.IndexOf(find, index, comparison);
                if (endIndex >= 0) {
                    result.Append(text.Substring(index, endIndex - index));
                    result.Append(replace);
                    index = endIndex + find.Length;
                }
                else {
                    result.Append(text.Substring(index));
                    break;
                }
            }
            return result.ToString();
        }
    }
}
