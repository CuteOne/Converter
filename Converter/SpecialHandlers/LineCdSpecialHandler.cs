using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.SpecialHandlers
{
    internal class LineCdSpecialHandler : BaseSpecialHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.SpecialHandling.Contains("line_cd");
        }
        int lineCount = 0;

        public override void Handle()
        {

            List<string> specialHandling = SplitSpecialHandling();
            foreach (var entry in specialHandling)
            {
                if (entry.Contains("line_cd="))
                {
                    int lineCdValue = int.Parse(entry.Replace("line_cd=", ""));
                    string actionName = StringUtilities.ConvertToTitleCaseNoSpace(Program.currentActionLine.Action);
                    ModifyConditions.Add(Program.currentActionLine, $"linecd.{actionName}{lineCount++}.{lineCdValue}");
                }
            }
            
        }
    }
}
