namespace SimcToBrConverter.ActionLines
{
    public class ActionLine : IParseResult
    {
        public string ListName { get; set; }
        public string Action { get; set; }
        public string SpecialHandling { get; set; }
        public string Condition { get; set; }
        public string Comment { get; set; }
        public ActionType Type { get; set; }

        public ActionLine(string listName = "", string action = "", string specialHandling = "", string condition = "", string comment = "", ActionType type = ActionType.Default)
        {
            ListName = listName;
            Action = action;
            SpecialHandling = specialHandling;
            Condition = condition;
            Comment = comment;
            Type = type;
        }
    }
}
