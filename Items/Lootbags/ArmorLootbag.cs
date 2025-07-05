namespace GodmistWPF.Items.Lootbags;

public class ArmorLootbag : Lootbag
{
    public override int ID => 572;
    public override string Alias => "ArmorLootbag";
    public ArmorLootbag(int level)
    {
        Level = level;
    }
}