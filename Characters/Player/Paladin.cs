using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;

namespace GodmistWPF.Characters.Player
{
    public class Paladin : PlayerCharacter {
        public override string Name { get; set; }
        // Mana
        // Capped at 120, cannot be increased
        // Start battle with full Mana, regenerates passively each turn by 20
        public Paladin(string name) : base(name, new Stat(450, 20),
            new Stat(21, 0.6), new Stat(30, 0.95),
            new Stat(0.08, 0), new Stat(8, 0.04),
            new Stat(16, 0.6), new Stat(16, 0.6),
            new Stat(35, 0), new Stat(0, 0),
            new Stat(1, 0), CharacterClass.Paladin) {
            Resistances.Add(StatusEffectType.Debuff, new Stat(0.25, 0));
            Resistances.Add(StatusEffectType.Bleed, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Poison, new Stat(0.4, 0));
            Resistances.Add(StatusEffectType.Burn, new Stat(0.5, 0));
            Resistances.Add(StatusEffectType.Stun, new Stat(0.75, 0));
            Resistances.Add(StatusEffectType.Freeze, new Stat(0.75, 0));
            Resistances.Add(StatusEffectType.Frostbite, new Stat(0.75, 0));
            Resistances.Add(StatusEffectType.Sleep, new Stat(0.75, 0));
            Resistances.Add(StatusEffectType.Paralysis, new Stat(0.75, 0));
            Resistances.Add(StatusEffectType.Provocation, new Stat(0.25, 0));
            _maximalResource = new Stat(120, 0);
            _resourceRegen = new Stat(20, 0);
            CurrentResource = MaximalResource;
            ResourceType = ResourceType.Mana;
            SwitchWeapon(new Weapon(CharacterClass.Paladin));
            SwitchArmor(new Armor(CharacterClass.Paladin));
            
            // Add some starting items to inventory
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Bandage(), 3);
            Inventory.AddItem(new GodmistWPF.Items.MiscItems.Antidote(), 2);
            
            ActiveSkills[0] = new ActiveSkill("Judgement", 0,0.6, false, 83,
            [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false, 0, 0)]);
            ActiveSkills[1] = new ActiveSkill("CrushingStrike", 30,0.7, false, 75,
                [new DealDamage(DamageType.Physical, DamageBase.Minimal, 1, true, false, 0, 0),
                    new DebuffResistance(SkillTarget.Enemy, StatusEffectType.Stun, ModifierType.Additive, 0.3, 0.9, 3),
                    new InflictGenericStatusEffect("Stun", 3, 0.5, "CrushingStrike")]);
            ActiveSkills[2] = new ActiveSkill("Cure", 50, 0.4, true, 100,
            [new HealTarget(SkillTarget.Self, 1.5, DamageBase.Random)]);
            ActiveSkills[3] = new ActiveSkill("HolyTransfusion", 60, 0.65, true, 100,
            [new DealDamage(DamageType.Magic, DamageBase.Random, 1, false, false, 1, 0)]);
            ActiveSkills[4] = new ActiveSkill("ShieldOfReflection", 40, 0.4,true, 100,
            [new BuffStat(SkillTarget.Self, StatType.Dodge, ModifierType.Multiplicative, 0.4, 1, 5)]);
        }
        public Paladin() {}
    }
}