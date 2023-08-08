﻿namespace SimcToBrConverter.Conditions
{
    public class ActionConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "action.";

        public override (string Result, bool Negate) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "ready":
                    result = $"cast.able.{spell}()";
                    break;
                default:
                    result = ""; // Unknown task
                    break;
            }

            return (result, negate);
        }
    }
}
