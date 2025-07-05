using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Items;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Characters.Player
{
    [JsonConverter(typeof(PlayerJsonConverter))]
    public abstract class PlayerCharacter : Character
    {
        public override double MinimalAttack
        {
            get => _minimalAttack.Value(this, "MinimalAttack") + (Weapon?.MinimalAttack?? 0);
            protected set => _minimalAttack.BaseValue = value;
        }
        public override double MaximalAttack
        {
            get => _maximalAttack.Value(this, "MaximalAttack") + (Weapon?.MaximalAttack?? 0);
            protected set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        public override double CritChance
        {
            get => Math.Clamp(_critChance.Value(this, "CritChance") + (Weapon?.CritChance?? 0), 0, 1);
            protected set => _critChance.BaseValue = value;
        }
        public override double CritMod
        {
            get => _critMod.Value(this, "CritMod") + (Weapon?.CritMod ?? 0);
            protected set => _critMod.BaseValue = value;
        }
        public override double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy") + (Weapon?.Accuracy ?? 0) - 
                   (ResourceType == ResourceType.Fury ? CurrentResource / 3 : 0);
            protected set => _accuracy.BaseValue = value;
        }
        public override double MaximalHealth
        {
            get => _maximalHealth.Value(this, "MaximalHealth") + (Armor?.MaximalHealth ?? 0);
            protected set => _maximalHealth.BaseValue = value;
        }

        public override double Dodge
        {
            get => _dodge.Value(this, "Dodge") + (Armor?.Dodge?? 0);
            protected set => _dodge.BaseValue = value;
        }

        public override double PhysicalDefense
        {
            get => _physicalDefense.Value(this, "PhysicalDefense") + (Armor?.PhysicalDefense?? 0);
            protected set => _physicalDefense.BaseValue = value;
        }

        public override double MagicDefense
        {
            get => _magicDefense.Value(this, "MagicDefense") + (Armor?.MagicDefense?? 0);
            protected set => _magicDefense.BaseValue = value;
        }
        protected PlayerCharacter(string name,
            Stat maxHealth,
            Stat minimalAttack,
            Stat maximalAttack,
            Stat critChance,
            Stat dodge,
            Stat physicalDefense,
            Stat magicDefense,
            Stat speed,
            Stat accuracy,
            Stat critMod,
            CharacterClass characterClass) : base(name, maxHealth, minimalAttack, maximalAttack, critChance,
            dodge, physicalDefense, magicDefense, speed, accuracy, critMod)
        {
            PassiveEffects = new PassiveEffectList();
            CharacterClass = characterClass;
            Resistances = new Dictionary<StatusEffectType, Stat>();
        }
        public PlayerCharacter() {}

        public CharacterClass CharacterClass { get; set; }
        public int Gold { get; set;} = 100;
        public int CurrentExperience { get; set; }
        public int RequiredExperience => CalculateExperience(Level);

        public int Honor {get; set;}

        public HonorLevel HonorLevel
        {
            get
            {
                return Honor switch
                {
                    < -100 => HonorLevel.Exile,
                    < -75 and >= -100 => HonorLevel.Useless,
                    < -50 and >= -75 => HonorLevel.Shameful,
                    < -20 and >= -50 => HonorLevel.Uncertain,
                    < 40 and >= -20 => HonorLevel.Recruit,
                    < 100 and >= 40 => HonorLevel.Mercenary,
                    < 150 and >= 100 => HonorLevel.Fighter,
                    < 200 and >= 150 => HonorLevel.Knight,
                    >= 200 => HonorLevel.Leader
                };
            }
        }
        public Inventory Inventory { get; set; } = new();
        public Weapon? Weapon { get; set; }
        public Armor? Armor { get; set; }

        public void SwitchWeapon(Weapon weapon)
        {
            if (Weapon != null) Inventory.AddItem(Weapon);
            Weapon = weapon;
            Weapon.UpdatePassives(this);
        }
        public void SwitchArmor(Armor armor)
        {
            if (Armor != null) Inventory.AddItem(Armor);
            Armor = armor;
            _currentHealth += armor.MaximalHealth;
            Armor.UpdatePassives(this);
        }

        public void GainGold(int gold) {
            Gold += (int)UtilityMethods.CalculateModValue(gold, PassiveEffects.GetModifiers("GoldGainMod"));
        }
        public void LoseGold(int gold) {
            Gold -= gold;
        }
        public void GainExperience(int experience)
        {
            var experienceGained = (int)UtilityMethods
                .CalculateModValue(experience * PlayerHandler.HonorExperienceModifier, PassiveEffects.GetModifiers("ExperienceGainMod"));
            CurrentExperience += experienceGained;
            while (CurrentExperience >= RequiredExperience) {
                if (Level < 50)
                {
                    Level++;
                    CurrentHealth = MaximalHealth;
                }
                else
                {
                    CurrentExperience = RequiredExperience;
                    return;
                }
            }
        }
        private int CalculateExperience(int level)
        {
            var value = 0;
            for (var i = 1; i <= Math.Min(level, 49); i++)
            {
                value += (int)(12 * Math.Pow(i, 1.5) + 12);
            }
            return value;
        }
        public void GainHonor(int honor)
        {
            var gain = Math.Min(200 - Honor, honor);
            Honor += gain;
        }
        public void LoseHonor(int honor) {
            var loss = Math.Min(Honor + 100, honor);
            Honor -= loss;
        }
    }
}