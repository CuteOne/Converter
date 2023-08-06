using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.Conditions
{
    public class BuffConditionConverter : IConditionConverter
    {
        public bool CanConvert(string condition)
        {
            return condition.StartsWith("buff.");
        }

        public string Convert(string condition)
        {
            var notFlag = condition.StartsWith("!") ? "not " : "";
            var spell = condition.Substring(condition.IndexOf("buff.") + 5);
            var formattedSpell = StringUtilities.ConvertToCamelCase(spell);
            return $"{notFlag}buff.{formattedSpell}.exists()";
        }
    }
}
