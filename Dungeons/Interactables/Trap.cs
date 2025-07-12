
using DamageType = GodmistWPF.Enums.DamageType;
using Difficulty = GodmistWPF.Enums.Difficulty;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Dungeons.Interactables;

/// <summary>
/// Klasa reprezentująca pułapkę w lochu, która może zadać obrażenia lub nałożyć efekty na gracza.
/// </summary>
public class Trap(Difficulty difficulty, DungeonField location, int trapType, DungeonType dungeonType)
{
    /// <summary>
    /// Pobiera poziom trudności pułapki.
    /// </summary>
    public Difficulty Difficulty { get; private set; } = difficulty;
    /// <summary>
    /// Pobiera typ pułapki.
    /// </summary>
    public int TrapType { get; private set; } = trapType;

    /// <summary>
    /// Pobiera lokalizację pułapki w lochu.
    /// </summary>
    public DungeonField Location { get; private set; } = location;
    /// <summary>
    /// Pobiera typ lochu, w którym znajduje się pułapka.
    /// </summary>
    public DungeonType DungeonType { get; private set; } = dungeonType;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Trap"/> z losowym typem pułapki.
    /// </summary>
    /// <param name="difficulty">Poziom trudności pułapki.</param>
    /// <param name="location">Lokalizacja pułapki w lochu.</param>
    /// <param name="dungeonType">Typ lochu, w którym znajduje się pułapka.</param>
    public Trap(Difficulty difficulty, DungeonField location, DungeonType dungeonType) : 
        this(difficulty, location, Random.Shared.Next(0, 3), dungeonType)
    {
    }

    /// <summary>
    /// Aktywuje pułapkę, zadając obrażenia i/lub nakładając efekty na gracza w zależności od typu lochu.
    /// </summary>
    public void Trigger()
    {
        switch (DungeonType)
        {
            case DungeonType.Catacombs:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10 , this);
                PlayerHandler.player.TakeDamage(DamageType.Physical, PlayerHandler.player.MaximalHealth / 5, this);
                break;
            case DungeonType.Forest:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                PlayerHandler.player.PassiveEffects.Add(StatusEffectFactory.CreateDoTEffect(PlayerHandler.player,
                    "Trap", "Poison", PlayerHandler.player.MaximalHealth / 25, 5, 1));
                break;
            case DungeonType.ElvishRuins:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                break;
            case DungeonType.Cove:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                PlayerHandler.player.PassiveEffects.Add(StatusEffectFactory.CreateDoTEffect(PlayerHandler.player,
                    "Trap", "Burn", PlayerHandler.player.MaximalHealth / 25, 5, 1));
                break;
            case DungeonType.Desert:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Additive, 15, "Trap", 5));
                break;
            case DungeonType.Temple:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10 , this);
                PlayerHandler.player.TakeDamage(DamageType.Magic, PlayerHandler.player.MaximalHealth / 5, this);
                break;
            case DungeonType.Mountains:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                PlayerHandler.player.PassiveEffects.Add(StatusEffectFactory.CreateDoTEffect(PlayerHandler.player,
                    "Trap", "Bleed", PlayerHandler.player.MaximalHealth / 25, 5, 1));
                break;
            case DungeonType.Swamp:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10, this);
                PlayerHandler.player.AddModifier(StatType.Speed, new StatModifier(ModifierType.Additive, 10, "Trap", 5));
                break;
        }
    }
}