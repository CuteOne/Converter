namespace SimcToBrConverter.Conditions
{
    public class UnitConditionConverter : IConditionConverter
    {
        public bool CanConvert(string condition)
        {
            return condition.StartsWith("target.time_to_die");
        }

        public string Convert(string condition)
        {
            return "unit.ttd(var.minTtdBs)";
        }
    }

}
