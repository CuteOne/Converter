using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionLines
{
    public class ActionLineParser
    {
        private static readonly Regex ActionPattern = new(
            @"^actions\.?(?<listName>\w+)?(\+=/|=)(?<action>[^,]+)(?:(?!,if=|,value=|,op=),(?<specialHandling>[^,]+(?:,[^,]+)*?)(?=(?:,if=|,value=|,op=|$)))?(?:,(?:if=|value=|op=)(?<condition>.+))?$",
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
            var action = match.Groups["action"].Value;
            var specialHandling = match.Groups["specialHandling"].Value;
            var condition = match.Groups["condition"].Value;
            var comment = CommentPattern.Match(line).Groups["comment"].Value;

            if (string.IsNullOrEmpty(action))
                throw new InvalidOperationException($"Failed to group the action from the line: {line}");

            if (!action.Contains("use_item") && !action.Contains("variable") && specialHandling.Contains("name="))
            {
                if (specialHandling.Contains('|'))
                {
                    // Handle the special case with multiple name= values
                    var names = specialHandling.Split('|');
                    var actionLines = new List<ActionLine>();
                    foreach (var name in names)
                    {
                        string? itemName;
                        if (name.Contains("name="))
                        {
                            match = Regex.Match(name, @"name=(?<command>\w+)");
                            itemName = $"{action}.{match.Groups["command"].Value}";
                        }
                        else
                        {
                            itemName = name;
                        }
                        actionLines.Add(new ActionLine(listName, itemName, specialHandling.Replace($"name={itemName}", "").Trim(','), condition, comment));
                    }
                    return new MultipleActionLineResult(actionLines);
                }
                else
                {
                    match = Regex.Match(specialHandling, @"name=(?<command>\w+)");
                    action = $"{action}.{match.Groups["command"].Value}";
                    specialHandling = specialHandling.Replace($"name={action}", "").Trim(',');
                }
            }

            return new ActionLine(listName, action, specialHandling, condition, comment);
        }
    }
}
