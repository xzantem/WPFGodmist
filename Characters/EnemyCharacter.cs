using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Items.Drops;
using GodmistWPF.Utilities;

namespace GodmistWPF.Characters;

/// <summary>
/// Reprezentuje przeciwnika w grze, dziedziczącego po klasie bazowej Character.
/// </summary>
/// <remarks>
/// Klasa ta rozszerza podstawową funkcjonalność postaci o specyficzne cechy przeciwników,
/// takie jak typy wrogów, domyślna lokalizacja i tabela przedmiotów.
/// </remarks>
public class EnemyCharacter : Character
{
    /// <summary>
    /// Pobiera lub ustawia listę typów, do których należy przeciwnik.
    /// </summary>
    /// <value>Lista typów przeciwnika.</value>
    /// <remarks>
    /// Przeciwnik może należeć do wielu typów jednocześnie, co może wpływać na jego zachowanie
    /// i interakcje z umiejętnościami oraz efektami.
    /// </remarks>
    public List<EnemyType> EnemyType { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia alias używany do lokalizacji nazwy przeciwnika.
    /// </summary>
    /// <value>Alias używany do wyszukiwania zlokalizowanej nazwy.</value>
    public string Alias { get; set; }
    
    /// <summary>
    /// Pobiera zlokalizowaną nazwę przeciwnika na podstawie aliasu.
    /// Jeśli tłumaczenie nie jest dostępne, zwracany jest sam alias.
    /// </summary>
    /// <value>Zlokalizowana nazwa przeciwnika lub alias, jeśli tłumaczenie nie istnieje.</value>
    public override string Name
    {
        get => locale.ResourceManager.GetString(Alias) ?? Alias;
        set {}
    }

    /// <summary>
    /// Pobiera lub ustawia domyślną lokalizację, w której występuje przeciwnik.
    /// </summary>
    /// <value>Typ lokacji, w której przeciwnik występuje domyślnie.</value>
    public DungeonType DefaultLocation { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia tabelę przedmiotów, które mogą zostać upuszczone przez przeciwnika.
    /// </summary>
    /// <value>Tabela przedmiotów zawierająca listę możliwych łupów.</value>
    public DropTable DropTable { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="EnemyCharacter"/>. Konstruktor bezparametrowy używany do deserializacji JSON.
    /// </summary>
    /// <remarks>
    /// Ten konstruktor jest wymagany przez mechanizm serializacji/deserializacji JSON.
    /// Nie należy go używać bezpośrednio do tworzenia nowych instancji przeciwników.
    /// </remarks>
    public EnemyCharacter()
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="EnemyCharacter"/> jako kopię innego przeciwnika z dostosowaniem poziomu.
    /// </summary>
    /// <param name="other">Przeciwnik, na podstawie którego tworzona jest kopia.</param>
    /// <param name="level">Poziom, na który mają być przeskalowane statystyki przeciwnika.</param>
    /// <remarks>
    /// <para>Konstruktor tworzy głęboką kopię przeciwnika, uwzględniając:</para>
    /// <list type="bullet">
    /// <item>Podstawowe właściwości (alias, typy, lokalizacja)</item>
    /// <item>Tabelę przedmiotów (głęboka kopia)</item>
    /// <item>Wszystkie statystyki z uwzględnieniem poziomu trudności gry</item>
    /// <item>Odporności na efekty statusowe</item>
    /// <item>Aktywne umiejętności (płytka kopia tablicy)</item>
    /// </list>
    /// <para>Statystyki są skalowane w zależności od wybranego poziomu trudności gry.</para>
    /// </remarks>
    public EnemyCharacter(EnemyCharacter other, int level)
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