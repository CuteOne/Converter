using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionLines
{
    public class ActionLine : IParseResult
    {
        public string ListName { get; set; }
        public string Action { get; set; }
        public string SpecialHandling { get; set; }
        public string Value { get; set; }
        public string Op { get; set; }
        public string Condition { get; set; }
        public string Comment { get; set; }
        public ActionType Type { get; set; }
        public string ConvertedSpecial { get; set; }
        public ActionType TypeSpecial { get; set; }
        public List<string> Conditions { get; set; }

        public ActionLine(string listName = "", string action = "", string specialHandling = "", string value = "", string op = "", string condition = "", string comment = "",
            ActionType type = ActionType.Default, string convertedSpecial = "", ActionType typeSpecial = ActionType.Default, List<string>? conditions = null)
        {
            ListName = listName;
            Action = action;
            SpecialHandling = specialHandling;
            Value = value;
            Op = op;
            Condition = condition;
            Comment = comment;
            Type = type;
            ConvertedSpecial = convertedSpecial;
            TypeSpecial = typeSpecial;
            Conditions = conditions ?? new List<String>();
        }
    }
}
