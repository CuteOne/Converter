namespace SimcToBrConverter.ActionLines
{
    public readonly struct ActionLine : IParseResult
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
}
