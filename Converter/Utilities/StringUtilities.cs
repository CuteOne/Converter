using System.Globalization;

namespace SimcToBrConverter.Utilities
{
    public static class StringUtilities
    {
        public static string ConvertToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            string[] words = input.Split('_');
            return words[0] + string.Join("", words.Skip(1).Select(w => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(w)));
        }

        public static string ConvertToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) { return string.Empty; }
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Replace('_', ' '));
        }

        public static string ConvertToTitleCaseNoSpace(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower()).Replace("_", "");
        }

        public static string CheckForOr(string input)
        {
            if (input.Contains("|") || input.Contains(" or "))
            {
                int indexOfPipe = -1;
                if (input.Contains("|"))
                    indexOfPipe = input.IndexOf("|");
                if (input.Contains(" or "))
                    indexOfPipe = input.IndexOf(" or ");
                if (indexOfPipe > -1)
                {
                    //bool hasOpeningParenthesisBeforePipe = input.Substring(0, indexOfPipe).Contains("(");
                    //bool hasClosingParenthesisAfterPipe = input.Substring(indexOfPipe).Contains(")");

                    //if (!hasOpeningParenthesisBeforePipe || !hasClosingParenthesisAfterPipe)
                    //{
                        input = $"({input})";
                    //}
                }
            }

            return input;
        }
    }
}
