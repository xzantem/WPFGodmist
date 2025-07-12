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
    /// <summary>
    /// Abstrakcyjna klasa bazowa reprezentująca postać gracza w grze.
    /// </summary>
    /// <remarks>
    /// Klasa dziedziczy po klasie <see cref="Character"/> i rozszerza jej funkcjonalność o mechanizmy
    /// specyficzne dla postaci sterowanych przez gracza, takie jak ekwipunek, doświadczenie, złoto i honory.
    /// </remarks>
    [JsonConverter(typeof(PlayerJsonConverter))]
    public abstract class PlayerCharacter : Character
    {
        /// <summary>
        /// Pobiera lub ustawia minimalną wartość ataku postaci.
        /// </summary>
        /// <value>Wartość minimalnego ataku uwzględniająca statystyki z ekwipunku.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej wartości ataku postaci i wartości z wyposażonej broni.
        /// </remarks>
        public override double MinimalAttack
        {
            get => _minimalAttack.Value(this, "MinimalAttack") + (Weapon?.MinimalAttack?? 0);
            protected set => _minimalAttack.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia maksymalną wartość ataku postaci.
        /// </summary>
        /// <value>Wartość maksymalnego ataku uwzględniająca statystyki z ekwipunku.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej wartości ataku postaci i wartości z wyposażonej broni.
        /// Zawsze jest większa lub równa wartości <see cref="MinimalAttack"/>.
        /// </remarks>
        public override double MaximalAttack
        {
            get => _maximalAttack.Value(this, "MaximalAttack") + (Weapon?.MaximalAttack?? 0);
            protected set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        /// <summary>
        /// Pobiera lub ustawia szansę na trafienie krytyczne.
        /// </summary>
        /// <value>Wartość z przedziału [0, 1] reprezentująca szansę na trafienie krytyczne.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej szansy na trafienie krytyczne i wartości z wyposażonej broni.
        /// Jest ograniczona do przedziału [0, 1].
        /// </remarks>
        public override double CritChance
        {
            get => Math.Clamp(_critChance.Value(this, "CritChance") + (Weapon?.CritChance?? 0), 0, 1);
            protected set => _critChance.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia mnożnik obrażeń przy trafieniu krytycznym.
        /// </summary>
        /// <value>Mnożnik obrażeń krytycznych.</value>
        /// <remarks>
        /// Wartość jest sumą bazowego mnożnika i wartości z wyposażonej broni.
        /// </remarks>
        public override double CritMod
        {
            get => _critMod.Value(this, "CritMod") + (Weapon?.CritMod ?? 0);
            protected set => _critMod.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia celność ataku postaci.
        /// </summary>
        /// <value>Wartość celności uwzględniająca statystyki z ekwipunku i modyfikatory.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej celności i wartości z wyposażonej broni.
        /// W przypadku postaci używających furii (ResourceType.Fury), celność jest zmniejszana proporcjonalnie do poziomu wściekłości.
        /// </remarks>
        public override double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy") + (Weapon?.Accuracy ?? 0) - 
                   (ResourceType == ResourceType.Fury ? CurrentResource / 3 : 0);
            protected set => _accuracy.BaseValue = value;
        }
        /// <summary>
        /// Pobiera lub ustawia maksymalną ilość punktów zdrowia postaci.
        /// </summary>
        /// <value>Maksymalna ilość punktów zdrowia uwzględniająca statystyki z pancerza.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej ilości zdrowia i wartości z wyposażonego pancerza.
        /// </remarks>
        public override double MaximalHealth
        {
            get => _maximalHealth.Value(this, "MaximalHealth") + (Armor?.MaximalHealth ?? 0);
            protected set => _maximalHealth.BaseValue = value;
        }

        /// <summary>
        /// Pobiera lub ustawia szansę na uniknięcie ataku.
        /// </summary>
        /// <value>Wartość szansy na unik uwzględniająca statystyki z pancerza.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej szansy na unik i wartości z wyposażonego pancerza.
        /// </remarks>
        public override double Dodge
        {
            get => _dodge.Value(this, "Dodge") + (Armor?.Dodge?? 0);
            protected set => _dodge.BaseValue = value;
        }

        /// <summary>
        /// Pobiera lub ustawia wartość obrony fizycznej.
        /// </summary>
        /// <value>Wartość obrony fizycznej uwzględniająca statystyki z pancerza.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej obrony fizycznej i wartości z wyposażonego pancerza.
        /// </remarks>
        public override double PhysicalDefense
        {
            get => _physicalDefense.Value(this, "PhysicalDefense") + (Armor?.PhysicalDefense?? 0);
            protected set => _physicalDefense.BaseValue = value;
        }

        /// <summary>
        /// Pobiera lub ustawia wartość obrony magicznej.
        /// </summary>
        /// <value>Wartość obrony magicznej uwzględniająca statystyki z pancerza.</value>
        /// <remarks>
        /// Wartość jest sumą bazowej obrony magicznej i wartości z wyposażonego pancerza.
        /// </remarks>
        public override double MagicDefense
        {
            get => _magicDefense.Value(this, "MagicDefense") + (Armor?.MagicDefense?? 0);
            protected set => _magicDefense.BaseValue = value;
        }
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="PlayerCharacter"/> z określonymi parametrami.
        /// </summary>
        /// <param name="name">Imię postaci.</param>
        /// <param name="maxHealth">Maksymalna ilość zdrowia.</param>
        /// <param name="minimalAttack">Minimalna wartość ataku.</param>
        /// <param name="maximalAttack">Maksymalna wartość ataku.</param>
        /// <param name="critChance">Szansa na trafienie krytyczne.</param>
        /// <param name="dodge">Szansa na uniknięcie ataku.</param>
        /// <param name="physicalDefense">Obrona fizyczna.</param>
        /// <param name="magicDefense">Obrona magiczna.</param>
        /// <param name="speed">Szybkość postaci.</param>
        /// <param name="accuracy">Celność ataku.</param>
        /// <param name="critMod">Mnożnik obrażeń krytycznych.</param>
        /// <param name="characterClass">Klasa postaci.</param>
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
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="PlayerCharacter"/> bez parametrów.
        /// </summary>
        /// <remarks>
        /// Konstruktor używany głównie do deserializacji. Należy ustawić wszystkie wymagane właściwości ręcznie.
        /// </remarks>
        public PlayerCharacter() {}

        /// <summary>
        /// Pobiera lub ustawia klasę postaci.
        /// </summary>
        /// <value>Wartość wyliczeniowa <see cref="CharacterClass"/> reprezentująca klasę postaci.</value>
        public CharacterClass CharacterClass { get; set; }
        /// <summary>
        /// Pobiera lub ustawia ilość posiadanego złota.
        /// </summary>
        /// <value>Ilość złota w ekwipunku postaci. Domyślna wartość to 100.</value>
        public int Gold { get; set;} = 100;
        /// <summary>
        /// Pobiera lub ustawia aktualną ilość punktów doświadczenia.
        /// </summary>
        /// <value>Liczba punktów doświadczenia potrzebnych do osiągnięcia następnego poziomu.</value>
        public int CurrentExperience { get; set; }
        /// <summary>
        /// Pobiera ilość doświadczenia wymaganą do osiągnięcia następnego poziomu.
        /// </summary>
        /// <value>Całkowita ilość doświadczenia potrzebna do osiągnięcia następnego poziomu.</value>
        public int RequiredExperience => CalculateExperience(Level);

        /// <summary>
        /// Pobiera lub ustawia wartość honoru postaci.
        /// </summary>
        /// <value>Wartość honoru w zakresie od -100 do 200.</value>
        /// <remarks>
        /// Wartość honoru wpływa na poziom honoru postaci i dostępne funkcje w grze.
        /// </remarks>
        public int Honor {get; set;}

        /// <summary>
        /// Pobiera poziom honoru postaci na podstawie wartości <see cref="Honor"/>.
        /// </summary>
        /// <value>Wartość wyliczeniowa <see cref="HonorLevel"/> reprezentująca poziom honoru.</value>
        /// <remarks>
        /// Poziom honoru jest określany na podstawie następujących progów:
        /// <list type="bullet">
        /// <item>Poniżej -100: <see cref="HonorLevel.Exile"/></item>
        /// <item>-100 do -76: <see cref="HonorLevel.Useless"/></item>
        /// <item>-75 do -51: <see cref="HonorLevel.Shameful"/></item>
        /// <item>-50 do -21: <see cref="HonorLevel.Uncertain"/></item>
        /// <item>-20 do 39: <see cref="HonorLevel.Recruit"/></item>
        /// <item>40 do 99: <see cref="HonorLevel.Mercenary"/></item>
        /// <item>100 do 149: <see cref="HonorLevel.Fighter"/></item>
        /// <item>150 do 199: <see cref="HonorLevel.Knight"/></item>
        /// <item>200 i więcej: <see cref="HonorLevel.Leader"/></item>
        /// </list>
        /// </remarks>
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
        /// <summary>
        /// Pobiera lub ustawia ekwipunek postaci.
        /// </summary>
        /// <value>Instancja <see cref="Inventory"/> reprezentująca ekwipunek postaci.</value>
        public Inventory Inventory { get; set; } = new();
        /// <summary>
        /// Pobiera lub ustawia wyposażoną broń.
        /// </summary>
        /// <value>Instancja <see cref="Weapon"/> reprezentująca wyposażoną broń lub null, jeśli postać nie ma broni.</value>
        public Weapon? Weapon { get; set; }
        /// <summary>
        /// Pobiera lub ustawia wyposażony pancerz.
        /// </summary>
        /// <value>Instancja <see cref="Armor"/> reprezentująca wyposażony pancerz lub null, jeśli postać nie ma pancerza.</value>
        public Armor? Armor { get; set; }

        /// <summary>
        /// Zmienia wyposażoną broń na podaną.
        /// </summary>
        /// <param name="weapon">Nowa broń do wyposażenia.</param>
        /// <remarks>
        /// Jeśli postać ma już wyposażoną broń, jest ona przenoszona do ekwipunku.
        /// Nowa broń jest wyposażana i jej efekty pasywne są aktywowane.
        /// </remarks>
        public void SwitchWeapon(Weapon weapon)
        {
            if (Weapon != null) Inventory.AddItem(Weapon);
            Weapon = weapon;
            Weapon.UpdatePassives(this);
        }
        /// <summary>
        /// Zmienia wyposażony pancerz na podany.
        /// </summary>
        /// <param name="armor">Nowy pancerz do wyposażenia.</param>
        /// <remarks>
        /// Jeśli postać ma już wyposażony pancerz, jest on przenoszony do ekwipunku.
        /// Nowy pancerz jest wyposażany, zwiększane jest maksymalne zdrowie postaci
        /// i aktywowane są jego efekty pasywne.
        /// </remarks>
        public void SwitchArmor(Armor armor)
        {
            if (Armor != null) Inventory.AddItem(Armor);
            Armor = armor;
            _currentHealth += armor.MaximalHealth;
            Armor.UpdatePassives(this);
        }

        /// <summary>
        /// Zwiększa ilość złota o określoną wartość, uwzględniając modyfikatory.
        /// </summary>
        /// <param name="gold">Podstawowa ilość złota do dodania.</param>
        /// <remarks>
        /// Rzeczywista ilość dodanego złota może zostać zmodyfikowana przez efekty pasywne.
        /// </remarks>
        public void GainGold(int gold) {
            Gold += (int)UtilityMethods.CalculateModValue(gold, PassiveEffects.GetModifiers("GoldGainMod"));
        }
        /// <summary>
        /// Zmniejsza ilość złota o określoną wartość.
        /// </summary>
        /// <param name="gold">Ilość złota do odjęcia.</param>
        /// <remarks>
        /// Jeśli ilość złota spadnie poniżej zera, zostanie ustawiona na zero.
        /// </remarks>
        public void LoseGold(int gold) {
            Gold = Math.Max(0, Gold - gold);
        }
        /// <summary>
        /// Zwiększa doświadczenie postaci o określoną wartość, uwzględniając modyfikatory.
        /// </summary>
        /// <param name="experience">Podstawowa ilość doświadczenia do dodania.</param>
        /// <remarks>
        /// Rzeczywista ilość dodanego doświadczenia jest modyfikowana przez:
        /// <list type="bullet">
        /// <item>Modyfikator doświadczenia z poziomu honoru (PlayerHandler.HonorExperienceModifier)</item>
        /// <item>Efekty pasywne zwiększające zysk doświadczenia</item>
        /// </list>
        /// Po przekroczeniu wymaganego doświadczenia, poziom postaci wzrasta, a zdrowie jest przywracane do maksimum.
        /// Maksymalny poziom to 50.
        /// </remarks>
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
        /// <summary>
        /// Oblicza ilość doświadczenia potrzebną do osiągnięcia określonego poziomu.
        /// </summary>
        /// <param name="level">Poziom, dla którego obliczane jest wymagane doświadczenie.</param>
        /// <returns>Całkowita ilość doświadczenia potrzebna do osiągnięcia podanego poziomu.</returns>
        /// <remarks>
        /// Formuła obliczeniowa: suma od 1 do poziomu z (12 * poziom^1.5 + 12)
        /// Maksymalny poziom uwzględniany w obliczeniach to 49.
        /// </remarks>
        private int CalculateExperience(int level)
        {
            var value = 0;
            for (var i = 1; i <= Math.Min(level, 49); i++)
            {
                value += (int)(12 * Math.Pow(i, 1.5) + 12);
            }
            return value;
        }
        /// <summary>
        /// Zwiększa wartość honoru o określoną ilość, nie przekraczając maksymalnej wartości 200.
        /// </summary>
        /// <param name="honor">Ilość honoru do dodania.</param>
        /// <remarks>
        /// Jeśli suma aktualnego i dodawanego honoru przekroczy 200, honor zostanie ustawiony na 200.
        /// </remarks>
        public void GainHonor(int honor)
        {
            var gain = Math.Min(200 - Honor, honor);
            Honor += gain;
        }
        /// <summary>
        /// Zmniejsza wartość honoru o określoną ilość, nie schodząc poniżej -100.
        /// </summary>
        /// <param name="honor">Ilość honoru do odjęcia.</param>
        /// <remarks>
        /// Jeśli różnica aktualnego i odejmowanego honoru spadnie poniżej -100, honor zostanie ustawiony na -100.
        /// </remarks>
        public void LoseHonor(int honor) {
            var loss = Math.Min(Honor + 100, honor);
            Honor -= loss;
        }
    }
}