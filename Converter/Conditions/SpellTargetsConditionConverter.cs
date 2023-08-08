namespace SimcToBrConverter.Conditions
{
    public class SpellTargetsConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "spell_targets.";

        protected override (string Result, bool Negate) ConvertTask(string spell, string task)
        {
            string result;
            bool negate = false;
            result = $"#enemies.yards0"; // Using 0 as a placeholder for the range

            return (result, negate);
        }
    }
}
