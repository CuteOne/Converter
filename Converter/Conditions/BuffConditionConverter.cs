﻿namespace SimcToBrConverter.Conditions
{
    public class BuffConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "buff.";

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "up":
                case "react":
                    result = $"buff.{spell}.exists()";
                    break;
                case "down":
                    result = $"buff.{spell}.exists()";
                    negate = true; // Reverse the negation for "down"
                    break;
                case "remains":
                    result = $"buff.{spell}.remains()";
                    break;
                case "stack":
                case "value":
                    result = $"buff.{spell}.count()";
                    break;
                default:
                    result = ""; // Unknown task
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
