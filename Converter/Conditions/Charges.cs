using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.Conditions
{
    public class ChargesConditionConverter : IConditionConverter
    {
        public bool CanConvert(string condition)
        {
            return condition.StartsWith("cooldown.") && condition.Contains(".full_recharge_time");
        }

        public string Convert(string condition)
        {
            var ability = condition.Substring(9, condition.IndexOf(".full_recharge_time") - 9);
            var formattedAbility = StringUtilities.ConvertToCamelCase(ability);
            return $"charges.{formattedAbility}.timeTillFull()";
        }
    }
}
