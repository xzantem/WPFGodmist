using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using Newtonsoft.Json;

namespace GodmistWPF.Characters.Player
{
    /// <summary>
    /// Klasa reprezentująca postać Maga w grze.
    /// </summary>
    /// <remarks>
    /// Mag to klasa używająca Many jako głównego zasobu, specjalizująca się w potężnych zaklęciach ofensywnych i defensywnych.
    /// Posiada wysokie obrażenia magiczne, ale mniejszą wytrzymałość fizyczną.
    /// </remarks>
    public class Sorcerer : PlayerCharacter {
        /// <summary>
        /// Pobiera lub ustawia imię postaci.
        /// </summary>
        /// <value>Imię postaci Maga.</value>
        public override string Name { get; set; }
        
        /// <summary>
        /// Pobiera maksymalną ilość zasobu (Many) postaci.
        /// </summary>
        /// <value>Maksymalna ilość Many, zwiększana przez statystyki broni.</value>
        [JsonIgnore]
        public override double MaximalResource
        {
            get => _maximalResource.Value(this, "MaximalResource") + Weapon?.Accuracy?? 0;
            protected set => _maximalResource.BaseValue = value;
        }
        [JsonIgnore]
        /// <summary>
        /// Pobiera lub ustawia regenerację zasobu (Many) na turę.
        /// </summary>
        /// <remarks>
        /// Dla Many: wartość bazowa + premia z krytycznego trafienia z broni.
        /// Dla Momentum: zależy od prędkości postaci.
        /// </remarks>
        public override double ResourceRegen
        {
            get
            {
                if (ResourceType == ResourceType.Momentum) return Speed >= 100 ? Speed / 10 : Speed / 5;
                return _resourceRegen.Value(this, "ResourceRegen") + Weapon?.CritChance?? 0;
            }
            set => _resourceRegen.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia szansę na trafienie krytyczne.
        /// </summary>
        /// <value>Szansa na trafienie krytyczne w procentach.</value>
        public override double CritChance
        {
            get => _critChance.Value(this, "CritChance");
            protected set => _critChance.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia celność postaci.
        /// </summary>
        /// <value>Wartość celności wpływająca na szansę trafienia.</value>
        public override double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy");
            protected set => _accuracy.BaseValue = value;
        }

        // Mana
        // Capped at 120, increased through various means, such as weapons or galdurites or potions
        // Start battle with full Mana, regenerates passively each turn by 15 (also can be increased)
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Sorcerer"/> o podanym imieniu.
        /// </summary>
        /// <param name="name">Imię Maga.</param>
        /// <remarks>
        /// <para>Zasób specjalny: Mana (maks. 120)</para>
        /// <list type="bullet">
        /// <item>Rozpoczyna walkę z pełną maną</item>
        /// <item>Pasywna regeneracja: 15 Many na turę (możliwa do zwiększenia)</li>
        /// <item>Maksymalna ilość Many może być zwiększana przez przedmioty i ulepszenia</li>
        /// </list>
        /// </remarks>
        public Sorcerer(string name) : base(name, new Stat(250, 7.5),
            new Stat(27, 1), new Stat(36, 1.3),
            new Stat(0, 0), new Stat(6, 0.04),
            new Stat(6, 0.25), new Stat(12, 0.5),
            new Stat(45, 0), new Stat(100, 0),
            new Stat(1, 0), CharacterClass.Sorcerer) {
            Resistances.Add(StatusEffectType.Debuff, new Stat(0.7, 0));
            Resistances.Add(StatusEffectType.Bleed, new Stat(0.25, 0));
            Resistances.Add(StatusEffectType.Poison, new Stat(0.25, 0));
            Resistances.Add(StatusEffectType.Burn, new Stat(0.8, 0));
            Resistances.Add(StatusEffectType.Stun, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Freeze, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Frostbite, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Sleep, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Paralysis, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Provocation, new Stat(0.7, 0));
            _maximalResource = new Stat(80, 0);
            _resourceRegen = new Stat(15, 0);
            CurrentResource = MaximalResource;
            ResourceType = ResourceType.Mana;
            SwitchWeapon(new Weapon(CharacterClass.Sorcerer));
            SwitchArmor(new Armor(CharacterClass.Sorcerer));
            
            // Add some starting items to inventory
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Bandage(), 3);
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Antidote(), 2);
            
            ActiveSkills[0] = new ActiveSkill("EnergyOrb", 5, 0.65, true, 100,
            [new DealDamage(DamageType.Magic, DamageBase.Random, 1, false, false, 0, 0)]);
            ActiveSkills[1] = new ActiveSkill("Fireball", 60, 0.65, true, 100,
                [new DealDamage(DamageType.Magic, DamageBase.Random, 1, false, false, 0, 0),
                new InflictDoTStatusEffect(SkillTarget.Enemy, 3, 0.8, "Burn", 0.75)]);
            ActiveSkills[2] = new ActiveSkill("Focus", 0, 0.85, true, 100,
            [new TradeHealthForResource(SkillTarget.Self, 0.1, 2)]);
            ActiveSkills[3] = new ActiveSkill("MagicShield", 55, 0.3, true, 100,
            [new GainShield(SkillTarget.Self, DamageBase.Random, 1.25, 1, 4)]);
            ActiveSkills[4] = new ActiveSkill("ExhaustingSpells", 0, 0, true, 100,
            [new ToggleInnatePassiveEffect(SkillTarget.Self, "NoResourceRegen"), 
                new ToggleListenerPassiveEffect(SkillTarget.Self, "SlowOnHit", [ModifierType.Additive, 12, 0.7, 3])]);
        }
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Sorcerer"/> bez parametrów.
        /// Wymagane do deserializacji.
        /// </summary>
        public Sorcerer() {}
    }
}