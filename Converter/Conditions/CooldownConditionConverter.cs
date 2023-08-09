﻿namespace SimcToBrConverter.Conditions
{
    public class CooldownConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "cooldown.";

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "charges":
                    result = $"charges.{spell}.count()";
                    break;
                case "full_recharge_time":
                    result = $"charges.{spell}.timeTillFull()";
                    break;
                case "remains":
                    result = $"cd.{spell}.exists()";
                    break;
                default:
                    result = ""; // Return an empty string or handle the default case as needed
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}