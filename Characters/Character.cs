using ConsoleGodmist.TextService;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Characters
{
    public abstract class Character {
        public abstract string Name { get; set; }
        public Stat _maximalHealth;
        [JsonIgnore]
        public virtual double MaximalHealth
        {
            get => _maximalHealth.Value(this, "MaximalHealth");
            protected set => _maximalHealth.BaseValue = value;
        }
        public double _currentHealth;   
        [JsonIgnore]
        public double CurrentHealth {
            get => _currentHealth;
            protected set => _currentHealth = Math.Clamp(value, 0, MaximalHealth);
        }
        public Stat _minimalAttack;
        [JsonIgnore]
        public virtual double MinimalAttack {
            get => _minimalAttack.Value(this, "MinimalAttack");
            protected set => _minimalAttack.BaseValue = value;
        }
        public Stat _maximalAttack;
        [JsonIgnore]
        public virtual double MaximalAttack { 
            get  => _maximalAttack.Value(this, "MaximalAttack");
            protected set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        public Stat _dodge;
        [JsonIgnore]
        public virtual double Dodge {
            get => _dodge.Value(this, "Dodge");
            protected set => _dodge.BaseValue = value;
        }
        public Stat _physicalDefense;
        [JsonIgnore]
        public virtual double PhysicalDefense {
            get => _physicalDefense.Value(this, "PhysicalDefense");
            protected set => _physicalDefense.BaseValue = value;
        }
        public Stat _magicDefense;
        [JsonIgnore]
        public virtual double MagicDefense
        {
            get => _magicDefense.Value(this, "MagicDefense");
            protected set => _magicDefense.BaseValue = value;
        }
        public Stat _critChance;
        [JsonIgnore]
        public virtual double CritChance
        {
            get => Math.Clamp(_critChance.Value(this, "CritChance"), 0, 1);
            protected set => _critChance.BaseValue = value;
        }
        public Stat _speed;
        [JsonIgnore]
        public double Speed
        {
            get => _speed.Value(this, "Speed") + (ResourceType == ResourceType.Momentum ? CurrentResource / 10 : 0);
            protected set => _speed.BaseValue = value;
        }

        public Stat _accuracy; 
        [JsonIgnore]
        public virtual double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy") - (ResourceType == ResourceType.Fury ? CurrentResource / 3 : 0);
            protected set => _accuracy.BaseValue = value;
        }
        public Stat _critMod; 
        [JsonIgnore]
        public virtual double CritMod
        {
            get => _critMod.Value(this, "CritMod");
            protected set => _critMod.BaseValue = value;
        }
        public Stat _maximalResource; 
        [JsonIgnore]
        public virtual double MaximalResource
        {
            get => _maximalResource.Value(this, "MaximalResource");
            protected set => _maximalResource.BaseValue = value;
        }
        protected double _currentResource;
        [JsonIgnore]
        public double CurrentResource {
            get => _currentResource;
            protected set => _currentResource = Math.Clamp(value, 0, MaximalHealth);
            
        }
        public Stat _resourceRegen;
        [JsonIgnore]
        public virtual double ResourceRegen
        {
            get
            {
                if (ResourceType == ResourceType.Momentum) return Speed >= 100 ? Speed / 10 : Speed / 5;
                return _resourceRegen.Value(this, "ResourceRegen");
            }
            set => _resourceRegen.BaseValue = value;
        }

        public ResourceType ResourceType { get; set; }
        public PassiveEffectList PassiveEffects { get; set; }
        public Dictionary<StatusEffectType, Stat> Resistances { get; set; }
        public int Level {get; set;}
        
        public ActiveSkill[] ActiveSkills { get; set; }

        protected Character() { }
        protected Character(string name, Stat maxHealth, Stat minimalAttack, Stat maximalAttack, 
            Stat critChance, Stat dodge, Stat physicalDefense, Stat magicDefense, Stat speed, Stat accuracy,
            Stat critMod, int level = 1) 
        {
            PassiveEffects = new PassiveEffectList();
            Name = name;
            _maximalHealth = maxHealth;
            CurrentHealth = maxHealth.BaseValue;
            _minimalAttack = minimalAttack;
            _maximalAttack = maximalAttack;
            _critChance = critChance;
            _dodge = dodge;
            _physicalDefense = physicalDefense;
            _magicDefense = magicDefense;
            _resourceRegen = new Stat(0, 0);
            _speed = speed;
            _accuracy = accuracy;
            _critMod = critMod;
            Level = level;
            ActiveSkills = new ActiveSkill[5];
        }
        public double TakeDamage(DamageType damageType, double damage, dynamic source)
        {
            if (damageType == DamageType.Magic &&
                Random.Shared.NextDouble() < UtilityMethods.CalculateModValue(0, 
                    PassiveEffects.GetModifiers("AbsorptionChance"))) return 0; //TODO: "DAMAGE ABSORBED!" or sth
            double damageTaken = DamageMitigated(damage, damageType, source);
            if (BattleManager.CurrentBattle != null)
                CharacterEventTextService.DisplayBattleTakeDamageText
                    (this, new Dictionary<DamageType, int> { { damageType, (int)damageTaken}});
            else
                CharacterEventTextService.DisplayTakeDamageText
                    (this, new Dictionary<DamageType, int> { {damageType, (int)damageTaken}});
            var shields = PassiveEffects.TimedEffects.Where(e => e.Type == "Shield").ToList();
            if (shields.Count > 0)
                damageTaken = StatusEffectHandler.TakeShieldsDamage(shields, this, damageTaken);
            CurrentHealth -= damageTaken;
            return damageTaken;
        }

        public void UseResource(int amount)
        {
            CurrentResource -= amount;
            if (ResourceType != ResourceType.Fury)
                CurrentResource = Math.Max(CurrentResource, 0);
        }

        public Stat? GetStat(string stat)
        {
            return stat switch
            {
                "MaximalHealth" => _maximalHealth,
                "MinimalAttack" => _minimalAttack,
                "MaximalAttack" => _maximalAttack,
                "CritChance" => _critChance,
                "Dodge" => _dodge,
                "PhysicalDefense" => _physicalDefense,
                "MagicDefense" => _magicDefense,
                "ResourceRegen" => _resourceRegen,
                "Speed" => _speed,
                "Accuracy" => _accuracy,
                "CritMod" => _critMod,
                "MaximalResource" => _maximalResource,
                _ => null
            };
        }

        public void RegenResource(int amount)
        {
            if (PassiveEffects.InnateEffects.Any(x => x.Type == "NoResourceRegen")) return;
            CurrentResource = Math.Min(CurrentResource + UtilityMethods.
                CalculateModValue(amount, PassiveEffects.GetModifiers("ResourceRegenMod")), MaximalResource);
            //CharacterEventTextService.DisplayResourceRegenText(this, amount);
        }

        public double DamageMitigated(double damage, DamageType damageType, dynamic source)
        {
            var armorPen = source is Character caster
                ? new Dictionary<DamageType, double> {
                    { DamageType.Physical, UtilityMethods.CalculateModValue(0, caster.PassiveEffects.GetModifiers("PhysicalArmorPen")) },
                    { DamageType.Magic, UtilityMethods.CalculateModValue(0, caster.PassiveEffects.GetModifiers("MagicArmorPen")) } }
                : new Dictionary<DamageType, double>
                { { DamageType.Physical, 0 }, { DamageType.Magic, 0 } };
            damage = damageType switch
            {
                DamageType.Physical => damage * damage / (damage + PhysicalDefense * (1 - armorPen[DamageType.Physical])),
                DamageType.Magic => damage * damage / (damage + MagicDefense * (1 - armorPen[DamageType.Physical])),
                DamageType.Bleed => damage * (1.5 - Resistances[StatusEffectType.Bleed].Value(this, "BleedResistance")),
                DamageType.Poison => damage * (1.5 - Resistances[StatusEffectType.Poison].Value(this, "PoisonResistance")),
                DamageType.Burn => damage * (1.5 - Resistances[StatusEffectType.Burn].Value(this, "BurnResistance")),
                _ => damage
            };
            var mods = new List<StatModifier>();
            mods.AddRange(PassiveEffects.GetModifiers("DamageTaken"));
            switch (damageType)
            {
                case DamageType.Physical:
                    mods.AddRange(PassiveEffects.GetModifiers("PhysicalDamageTaken"));
                    break;
                case DamageType.Magic:
                    mods.AddRange(PassiveEffects.GetModifiers("MagicDamageTaken"));
                    break;
                case DamageType.Bleed:
                    mods.AddRange(PassiveEffects.GetModifiers("BleedDamageTaken"));
                    break;
                case DamageType.Poison:
                    mods.AddRange(PassiveEffects.GetModifiers("PoisonDamageTaken"));
                    break;
                case DamageType.Burn:
                    mods.AddRange(PassiveEffects.GetModifiers("BurnDamageTaken"));
                    break;
            }
            damage = UtilityMethods.CalculateModValue(damage, mods);
            return Math.Max(damage, 1);
        }
        public void Heal(double heal) {
            CurrentHealth += heal;
            CharacterEventTextService.DisplayHealText(this, (int)heal);
        }

        public void AddModifier(StatType stat, StatModifier modifier)
        {
            switch (stat)
            {
                case StatType.MaximalHealth:
                    _maximalHealth.AddModifier(modifier);
                    break;
                case StatType.MinimalAttack:
                    _minimalAttack.AddModifier(modifier);
                    break;
                case StatType.MaximalAttack:
                    _maximalAttack.AddModifier(modifier);
                    break;
                case StatType.Dodge:
                    _dodge.AddModifier(modifier);
                    break;
                case StatType.PhysicalDefense:
                    _physicalDefense.AddModifier(modifier);
                    break;
                case StatType.MagicDefense:
                    _magicDefense.AddModifier(modifier);
                    break;
                case StatType.CritChance:
                    _critChance.AddModifier(modifier);
                    break;
                case StatType.Speed:
                    _speed.AddModifier(modifier);
                    break;
                case StatType.Accuracy:
                    _accuracy.AddModifier(modifier);
                    break;
                case StatType.MaximalResource:
                    _maximalResource.AddModifier(modifier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public void AddResistanceModifier(StatusEffectType stat, StatModifier modifier)
        {
            Resistances[stat].AddModifier(modifier);
        }
        

        public void HandleModifiers()
        {
            _maximalHealth.Tick();
            _minimalAttack.Tick();
            _maximalAttack.Tick();
            _dodge.Tick();
            _physicalDefense.Tick();
            _magicDefense.Tick();
            _accuracy.Tick();
            _speed.Tick();
            _maximalResource.Tick();
        }

        public Dictionary<StatModifier, StatType> GetModifiers()
        {
            var mods = _maximalHealth.Modifiers
                .ToDictionary(mod => mod, mod => StatType.MaximalHealth);
            foreach (var mod in _minimalAttack.Modifiers)
                mods.Add(mod, StatType.MinimalAttack);
            foreach (var mod in _maximalAttack.Modifiers)
                mods.Add(mod, StatType.MaximalAttack);
            foreach (var mod in _dodge.Modifiers)
                mods.Add(mod, StatType.Dodge);
            foreach (var mod in _physicalDefense.Modifiers)
                mods.Add(mod, StatType.PhysicalDefense);
            foreach (var mod in _magicDefense.Modifiers)
                mods.Add(mod, StatType.MagicDefense);
            foreach (var mod in _accuracy.Modifiers)
                mods.Add(mod, StatType.Accuracy);
            foreach (var mod in _speed.Modifiers)
                mods.Add(mod, StatType.Speed);
            foreach (var mod in _maximalResource.Modifiers)
                mods.Add(mod, StatType.MaximalResource);
            return mods;
        }
    }
}