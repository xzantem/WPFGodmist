using ConsoleGodmist;
using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Items.Drops;
using GodmistWPF.Utilities;

namespace GodmistWPF.Characters;

public class EnemyCharacter : Character
{
    public List<EnemyType> EnemyType { get; set; }
    public string Alias { get; set; }
    public override string Name
    {
        get => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
        set {}
    }

    public DungeonType DefaultLocation { get; set; }
    
    public DropTable DropTable { get; set; }

    public EnemyCharacter()
    {
    } // For JSON Serialization

    public EnemyCharacter(EnemyCharacter other, int level) // Deep Copy for initializing new monsters
    {
        PassiveEffects = new PassiveEffectList();
        Level = level;
        Alias = other.Alias;
        EnemyType = other.EnemyType;
        DefaultLocation = other.DefaultLocation;
        DropTable = new DropTable(other.DropTable.Table.ToList());
        var diffFactor = GameSettings.Difficulty switch
        {
            Difficulty.Easy => 0.75,
            Difficulty.Normal => 1,
            Difficulty.Hard => 1.25,
            Difficulty.Nightmare => 1.5,
            _ => throw new ArgumentOutOfRangeException(nameof(GameSettings.Difficulty), GameSettings.Difficulty, null)
        };
        _maximalHealth = new Stat(other._maximalHealth.BaseValue * diffFactor, other._maximalHealth.ScalingFactor * diffFactor);
        _minimalAttack = new Stat(other._minimalAttack.BaseValue * diffFactor, other._minimalAttack.ScalingFactor * diffFactor);
        _maximalAttack = new Stat(other._maximalAttack.BaseValue * diffFactor, other._maximalAttack.ScalingFactor * diffFactor);
        CurrentHealth = MaximalHealth;
        _critChance = new Stat(other._critChance.BaseValue * diffFactor, other._critChance.ScalingFactor * diffFactor);
        _dodge = new Stat(other._dodge.BaseValue * diffFactor, other._dodge.ScalingFactor * diffFactor);
        _physicalDefense = new Stat(other._physicalDefense.BaseValue * diffFactor, other._physicalDefense.ScalingFactor * diffFactor);
        _magicDefense = new Stat(other._magicDefense.BaseValue * diffFactor, other._magicDefense.ScalingFactor * diffFactor);
        _resourceRegen = new Stat(other._resourceRegen.BaseValue, other._resourceRegen.ScalingFactor);
        _maximalResource = new Stat(other._maximalResource.BaseValue, other._maximalResource.ScalingFactor);
        _currentResource = 0;
        ResourceType = other.ResourceType;
        _speed = new Stat(other._speed.BaseValue, other._speed.ScalingFactor);
        _accuracy = new Stat(other._accuracy.BaseValue, other._accuracy.ScalingFactor);
        _critMod = new Stat(other._critMod.BaseValue, other._critMod.ScalingFactor);
        Resistances = other.Resistances.ToDictionary(x => x.Key, x => 
            new Stat(x.Value.BaseValue, x.Value.ScalingFactor));
        ActiveSkills = (ActiveSkill[])other.ActiveSkills.Clone();
    }
}