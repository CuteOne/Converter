using System.Globalization;

namespace SimcToBrConverter.Utilities
{
    public static class StringUtilities
    {
        public static string ConvertToCamelCase(string input)
        {
            string[] words = input.Split('_');
            return words[0] + string.Join("", words.Skip(1).Select(w => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(w)));
        }

        public static string ConvertToTitleCase(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Replace('_', ' '));
        }
    }
}
