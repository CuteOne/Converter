namespace SimcToBrConverter.Utilities
{
    public static class SpellRepository
    {
        private static readonly Dictionary<string, HashSet<string>> spells = new();

        public static void AddSpell(string spellName, string type)
        {
            if (!spells.TryGetValue(spellName, out var types))
            {
                types = new HashSet<string>();
                spells[spellName] = types;
            }
            types.Add(type);
        }

        public static Dictionary<string, HashSet<string>> GetAllSpells()
        {
            return spells;
        }

        public static void PrintSpellsByType()
        {
            var spellsByType = spells
               .SelectMany(kvp => kvp.Value.Select(type => new { Type = type, Spell = kvp.Key }))
               .GroupBy(item => item.Type)
               .ToDictionary(group => group.Key, group => group.Select(item => item.Spell).OrderBy(spell => spell).ToList());

            // Disclaimer and Important Note
            Console.WriteLine(@"
--- THIS SECTION IS FOR YOUR INFORMATION ONLY, DO NOT PLACE IN PROFILE ---
--------------------------------------------------------------------------
--- NOTE: spells listed under abilities may also be talents, even if   ---
--- not listed as such here, when implementing in the spell list. If   ---
--- they are talents, list them in the talents section only, any       ---
--- usable talents are automatically added as abilities. Additional    ---
--- due to nature of the conversion not all spells listed here are     ---
--- actual spells, some may be combinations of spells. In such an      ---
--- event, do not add to the spell list. Also be sure to add spells    ---
--- according to where they would best fit: Global, Class Shared, Spec ---
--------------------------------------------------------------------------
            ");

            // Sort the types alphabetically and print spells by type
            Console.WriteLine("local spells = {");
            foreach (var typeEntry in spellsByType.OrderBy(t => t.Key))
            {
                Console.WriteLine($"    {typeEntry.Key} = {{");
                foreach (var spellName in typeEntry.Value)
                {
                    Console.WriteLine($"        {spellName},");
                }
                Console.WriteLine("    },");
            }
            Console.WriteLine("}");
        }

    }
}
