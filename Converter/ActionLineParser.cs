using System.Text.RegularExpressions;

namespace SimcToBrConverter
{
    public class ActionLineParser
    {
        private static readonly Regex ActionPattern = new Regex(
        @"^actions\.?(?<listName>\w+)?(\+=/|=)(?<action>[^,]+)(?:(?!,if=|,value=|,op=),(?<specialHandling>[^,]+))?(?:,(?:if=|value=|op=)(?<condition>.+))?$",
        RegexOptions.Compiled);

        public readonly struct ActionLine
        {
            public readonly string ListName;
            public readonly string Action;
            public readonly string SpecialHandling;
            public readonly string Condition;

            public ActionLine(string listName = "", string action = "", string specialHandling = "", string condition = "")
            {
                ListName = listName;
                Action = action;
                SpecialHandling = specialHandling;
                Condition = condition;
            }
        }

        public static ActionLine ParseActionLine(string line)
        {
            var match = ActionPattern.Match(line);
            if (!match.Success) return new ActionLine();

            var listName = match.Groups["listName"].Value.TrimStart('.');
            var action = match.Groups["action"].Value;
            var specialHandling = match.Groups["specialHandling"].Value;
            var condition = match.Groups["condition"].Value;


            return new ActionLine(listName, action, specialHandling, condition);
        }
    }
}
