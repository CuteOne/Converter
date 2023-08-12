using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionLines
{
    public class ActionLineParser
    {
        private static readonly Regex ActionPattern = new Regex(
        @"^actions\.?(?<listName>\w+)?(\+=/|=)(?<action>[^,]+)(?:(?!,if=|,value=|,op=),(?<specialHandling>[^,]+))?(?:,(?:if=|value=|op=)(?<condition>.+))?$",
        RegexOptions.Compiled);

        public static IParseResult ParseActionLine(string line)
        {
            var match = ActionPattern.Match(line);
            if (!match.Success) return new ActionLine();

            var listName = match.Groups["listName"].Value.TrimStart('.');
            var action = match.Groups["action"].Value;
            var specialHandling = match.Groups["specialHandling"].Value;
            var condition = match.Groups["condition"].Value;

            if (action.Contains("use_item"))
            {
                if (specialHandling.Contains("|"))
                {
                    // Handle the special case with multiple name= values
                    var names = specialHandling.Split('|');
                    var actionLines = new List<ActionLine>();
                    foreach (var name in names)
                    {
                        match = Regex.Match(name, @"name=(?<command>\w+),");
                        var itemName = match.Groups["command"].Value;
                        actionLines.Add(new ActionLine(listName, itemName, specialHandling, condition));
                    }
                    return new MultipleActionLineResult(actionLines);
                }
                else
                {
                    match = Regex.Match(specialHandling, @"name=(?<command>\w+),");
                    action = match.Groups["command"].Value;
                }
            }


            return new ActionLine(listName, action, specialHandling, condition);
        }
    }
}
