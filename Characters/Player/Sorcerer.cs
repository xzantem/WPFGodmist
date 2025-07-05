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
    public class Sorcerer : PlayerCharacter {
        public override string Name { get; set; }
        
        [JsonIgnore]
        public override double MaximalResource
        {
            get => _maximalResource.Value(this, "MaximalResource") + Weapon?.Accuracy?? 0;
            protected set => _maximalResource.BaseValue = value;
        }
        [JsonIgnore]
        public override double ResourceRegen
        {
            get
            {
                if (ResourceType == ResourceType.Momentum) return Speed >= 100 ? Speed / 10 : Speed / 5;
                return _resourceRegen.Value(this, "ResourceRegen") + Weapon?.CritChance?? 0;
            }
            set => _resourceRegen.BaseValue = value;
        }
        public override double CritChance
        {
            get => _critChance.Value(this, "CritChance");
            protected set => _critChance.BaseValue = value;
        }
        public override double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy");
            protected set => _accuracy.BaseValue = value;
        }

        // Mana
        // Capped at 120, increased through various means, such as weapons or galdurites or potions
        // Start battle with full Mana, regenerates passively each turn by 15 (also can be increased)
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
        public Sorcerer() {}
    }
}