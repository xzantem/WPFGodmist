using GodmistWPF.Towns.NPCs;

namespace GodmistWPF.Towns;

public static class TownsHandler
{
    public static Town Arungard { get; set; }

    public static NPC FindNPC(string alias)
    {
        return Arungard.NPCs.FirstOrDefault(x => x.Alias == alias);
    }
}