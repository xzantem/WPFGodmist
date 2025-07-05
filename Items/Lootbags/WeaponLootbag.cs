namespace GodmistWPF.Items.Lootbags;

public class WeaponLootbag : Lootbag
{
    public override string Alias => "WeaponLootbag";
    public WeaponLootbag(int level)
    {
        Level = level;
    }
}