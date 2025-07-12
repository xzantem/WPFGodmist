using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;

namespace GodmistWPF.Characters.Player
{
    /// <summary>
    /// Klasa reprezentująca postać Zwiadowcy w grze.
    /// </summary>
    /// <remarks>
    /// Zwiadowca to zwinna klasa, która wykorzystuje Momentum jako swój główny zasób.
    /// Zyskuje prędkość w miarę gromadzenia Momentum, co pozwala na szybsze wykonywanie akcji.
    /// Specjalizuje się w unikach i precyzyjnych atakach dystansowych.
    /// </remarks>
    public class Scout : PlayerCharacter {
        /// <summary>
        /// Pobiera lub ustawia imię postaci.
        /// </summary>
        /// <value>Imię postaci Zwiadowcy.</value>
        public override string Name { get; set; }
        // Momentum
        // Maximal is capped at 200
        // Each turn gain momentum equal to 20% of your speed, 10% if at 100 or above
        // Gain 1 speed for every 10 Momentum (up to 20)
        // As with all classes, used for casting skills, downside to using is losing bonus speed
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Scout"/> o podanym imieniu.
        /// </summary>
        /// <param name="name">Imię Zwiadowcy.</param>
        /// <remarks>
        /// <para>Zasób specjalny: Momentum (maks. 200)</para>
        /// <list type="bullet">
        /// <item>Każda tura: +20% prędkości jako Momentum (lub +10% jeśli Momentum ≥ 100)</item>
        /// <item>+1 do prędkości za każde 10 punktów Momentum (maks. +20)</item>
        /// <item>Użycie umiejętności zużywa Momentum, co może zmniejszyć bonus do prędkości</item>
        /// </list>
        /// </remarks>
        public Scout(string name) : base(name, new Stat(300, 10),
            new Stat(18, 0.8), new Stat(36, 1.1),
            new Stat(0.08, 0), new Stat(17, 0.08),
            new Stat(8, 0.33), new Stat(4, 0.2),
            new Stat(55, 0), new Stat(0, 0),
            new Stat(1, 0), CharacterClass.Scout) {
            Resistances.Add(StatusEffectType.Debuff, new Stat(0.5, 0));
            Resistances.Add(StatusEffectType.Bleed, new Stat(0.5, 0));
            Resistances.Add(StatusEffectType.Poison, new Stat(0.7, 0));
            Resistances.Add(StatusEffectType.Burn, new Stat(0.4, 0));
            Resistances.Add(StatusEffectType.Stun, new Stat(0.35, 0));
            Resistances.Add(StatusEffectType.Freeze, new Stat(0.35, 0));
            Resistances.Add(StatusEffectType.Frostbite, new Stat(0.35, 0));
            Resistances.Add(StatusEffectType.Sleep, new Stat(0.35, 0));
            Resistances.Add(StatusEffectType.Paralysis, new Stat(0.35, 0));
            Resistances.Add(StatusEffectType.Provocation, new Stat(0.5, 0));
            _maximalResource = new Stat(200, 0);
            CurrentResource = 0;
            ResourceType = ResourceType.Momentum;
            Speed = _speed.BaseValue;
            SwitchWeapon(new Weapon(CharacterClass.Scout));
            SwitchArmor(new Armor(CharacterClass.Scout));
            
            // Add some starting items to inventory
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Bandage(), 3);
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Antidote(), 2);
            
            ActiveSkills[0] = new ActiveSkill("DaggerThrow", 0, 0.6, false, 82,
            [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false, 0, 0)]);
            ActiveSkills[1] = new ActiveSkill("Exchange", 40, 0.4, false, 74,
                [new DebuffStat(SkillTarget.Enemy, StatType.Dodge, ModifierType.Additive, 15, 0.8, 3)]);
            ActiveSkills[2] = new ActiveSkill("CloudOfSmoke", 50, 0.3, false, 70,
                [new DebuffStat(SkillTarget.Enemy, StatType.Accuracy, ModifierType.Additive, 20, 0.8, 3)]);
            ActiveSkills[3] = new ActiveSkill("Hookshot", 25, 0.3, false, 80,
            [new DealDamage(DamageType.Physical, DamageBase.Minimal, 1, true, false, 0, 0),
                new InflictGenericStatusEffect("Stun", 2, 0.75, "Hookshot")]);
            ActiveSkills[4] = new ActiveSkill("AccurateThrow", 50, 0.7, true, 85,
            [new DealDamage(DamageType.Physical, DamageBase.Maximal, 1, true, true, 0, 0)]);
        }
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Scout"/> bez parametrów.
        /// Wymagane do deserializacji.
        /// </summary>
        public Scout() {}
    }
}