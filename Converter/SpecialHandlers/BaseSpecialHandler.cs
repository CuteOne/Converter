namespace SimcToBrConverter.SpecialHandlers
{
    public abstract class BaseSpecialHandler : ISpecialHandler
    {
        public abstract bool CanHandle();

        public abstract void Handle();

        public List<string> SplitSpecialHandling()
        {
            List<string> specialHandling = new();
            if (Program.currentActionLine.SpecialHandling.Contains(','))
                specialHandling = Program.currentActionLine.SpecialHandling.Split(',').ToList();
            else
                specialHandling.Add(Program.currentActionLine.SpecialHandling);

            return specialHandling;
        }
    }
}
