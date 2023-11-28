using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionLines
{
    public class ActionLineParser
    {
        private static readonly Regex ActionPattern = new(
            @"^actions\.?(?<listName>\w+)?(\+=/|=)(?<action>[^,]+)(?:(?!,if=|,value=|,op=),(?<specialHandling>[^,]+(?:,[^,]+)*?)(?=(?:,if=|,value=|,op=|$)))?(?:,value=(?<value>[^,]+))?(?:,op=(?<op>[^,]+))?(?:,if=(?<condition>.+))?$",
            RegexOptions.Compiled);
        private static readonly Regex CommentPattern = new(
            @"^actions\.?(?:\w+)?(?:(?=\+=/)|(?==))(?:\+=/|=)(?<comment>.+)$",
            RegexOptions.Compiled);

        public static IParseResult ParseActionLine(string line)
        {
            var match = ActionPattern.Match(line);
            if (!match.Success)
                throw new InvalidOperationException($"Failed to process the line: {line}");

            var listName = match.Groups["listName"].Value.TrimStart('.');
            if (string.IsNullOrEmpty(listName)) listName = "combat";
            var action = match.Groups["action"].Value;
            var specialHandling = match.Groups["specialHandling"].Value;
            var condition = match.Groups["condition"].Value;
            var value = match.Groups["value"].Value;
            var op = match.Groups["op"].Value;
            var comment = CommentPattern.Match(line).Groups["comment"].Value;

            if (string.IsNullOrEmpty(action))
                throw new InvalidOperationException($"Failed to group the action from the line: {line}");
            List<string> conditions = new();
            return new ActionLine(listName, action, specialHandling, value, op, condition, comment, ActionType.Default, "", ActionType.Default);
        }
    }
}
