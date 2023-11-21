using SimcToBrConverter.ActionLines;
using System.Text;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Utilities
{
    public static class ConditionConverterUtility
    {
        //private static readonly Regex SplitConditionRegex = new(@"([&|\(\)!]|<=|>=|<|>|=|!=|\+|\-|\*|/|%%|%|@|<\?|>\?|\b\d+\b)", RegexOptions.Compiled);
        private static readonly Regex SplitConditionRegex = new(@"([&|\(\)!]|<=|>=|<|>|=|!=|\+|\-|\*|/|%%|%|@|<\?|>\?|(?<!\.\d*)\b\d+\b(?!\.\d*))", RegexOptions.Compiled);
        private static readonly Regex ModulusRegex = new(@"(?<!%)%%", RegexOptions.Compiled);
        private static readonly Regex XorRegex = new(@"(?<=\b|\s|\()(?<x>[^&|^|^\!]+)\^(?<y>[^&|^|^\!]+)(?=\b|\s|\))", RegexOptions.Compiled);
        private static readonly Regex EqualityRegex = new(@"(?<![\!=<>])=(?![\!=<>])", RegexOptions.Compiled);

        /// <summary>
        /// Splits the provided condition string by the & and | symbols, and parentheses, keeping the delimiters.
        /// </summary>
        /// <param name="condition">The condition string to split.</param>
        /// <returns>A list of individual condition parts.</returns>
        public static List<string> SplitCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return new List<string>();
            }
            return SplitConditionRegex.Split(condition)
                                      .Select(part => part.Trim())
                                      .Where(trimmedPart => !string.IsNullOrEmpty(trimmedPart))
                                      .ToList();
        }

        public static string ConvertOperatorsToLua(string condition)
        {
            var builder = new StringBuilder(condition);
            ConvertLogicalOperatorsToLua(builder);
            ConvertArithmeticOperatorsToLua(builder);

            return builder.ToString();
        }

        private static void ConvertLogicalOperatorsToLua(StringBuilder builder)
        {
            builder.Replace("&", " and ")
                   .Replace("|", " or ")
                   .Replace("!", "not ");

            // Manually apply regex replacements
            string condition = builder.ToString();
            condition = EqualityRegex.Replace(condition, "==");
            condition = XorRegex.Replace(condition, m => $"({m.Groups["x"].Value} or {m.Groups["y"].Value}) and not ({m.Groups["x"].Value} and {m.Groups["y"].Value})");

            builder.Clear();
            builder.Append(condition);
        }

        private static void ConvertArithmeticOperatorsToLua(StringBuilder builder)
        {
            string condition = builder.ToString();

            // Use a unique placeholder for '%%' to avoid conflict during division replacement
            string modulusPlaceholder = "__MODULUS__";
            condition = ModulusRegex.Replace(condition, modulusPlaceholder);

            // Replace '%' with '/' for division
            condition = condition.Replace("%", "/");

            // Replace the modulus placeholder with '%' for LUA modulus
            condition = condition.Replace(modulusPlaceholder, "%");

            builder.Clear();
            builder.Append(condition)
                   .Replace("@", "math.abs")
                   .Replace("<?", "math.max")
                   .Replace(">?", "math.min")
                   .Replace("floor(", "math.floor(")
                   .Replace("ceil(", "math.ceil(");
        }

        public static bool IsOperatorOrNumber(string conditionPart)
        {
            return new[] { "&", "|", "(", ")", "!", "<=", ">=", "<", ">", "=", "!=", "+", "-", "*", "/", "%", "%%", "@", "<?", ">?", "floor(", "ceil(" }
                   .Contains(conditionPart)
                   || double.TryParse(conditionPart, out _);
        }
    }
}
