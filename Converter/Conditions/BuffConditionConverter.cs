using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.Conditions
{
    public class BuffConditionConverter : IConditionConverter
    {
        public bool CanConvert(string condition)
        {
            return condition.StartsWith("buff.") || condition.StartsWith("!buff.");
        }

        public string Convert(string condition)
        {
            var notFlag = condition.StartsWith("!") ? "not " : "";
            var spell = condition.Substring(condition.IndexOf("buff.") + 5).Split('.')[0];
            var task = condition.Substring(condition.IndexOf("buff.") + 5).Split('.')[1];
            var formattedSpell = StringUtilities.ConvertToCamelCase(spell);
            switch (task)
            {
                case "up":
                    return $"{notFlag}buff.{formattedSpell}.exists()";
                case "down":
                    if (notFlag == "not ")
                        return $"buff.{formattedSpell}.exists()";
                    else
                        return $"not buff.{formattedSpell}.exists()";
                case "remains":
                    return $"{notFlag}buff.{formattedSpell}.remains()";
                case "react":
                    return $"{notFlag}buff.{formattedSpell}.exists()";
                case "stack":
                    return $"{notFlag}buff.{formattedSpell}.count()";
                case "value":
                    return $"{notFlag}buff.{formattedSpell}.count()";
                default:
                    return "";
            }
        }
    }
}
