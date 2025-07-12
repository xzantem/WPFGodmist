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
    /// <summary>
    /// Abstrakcyjna klasa bazowa reprezentująca postać w grze.
    /// </summary>
    /// <remarks>
    /// <para>Klasa dostarcza podstawowe właściwości i mechanizmy wspólne dla wszystkich postaci w grze, w tym:</para>
    /// <list type="bullet">
    /// <item>Statystyki postaci (życie, atak, obrona itp.)</item>
    /// <item>Mechanikę obrażeń i leczenia</item>
    /// <item>System modyfikatorów i efektów</item>
    /// <item>Zarządzanie umiejętnościami</item>
    /// </list>
    /// <para>Klasa jest dziedziczona przez <see cref="PlayerCharacter"/>, <see cref="EnemyCharacter"/> i inne typy postaci.</para>
    /// </remarks>
    public abstract class Character {
        /// <summary>
        /// Pobiera lub ustawia nazwę postaci.
        /// </summary>
        /// <value>
        /// Nazwa postaci widoczna w interfejsie użytkownika.
        /// </value>
        public abstract string Name { get; set; }
        /// <summary>
        /// Pole wspierające dla właściwości MaximalHealth.
        /// Przechowuje bazową wartość maksymalnego zdrowia postaci.
        /// </summary>
        public Stat _maximalHealth;
        
        /// <summary>
        /// Pobiera lub ustawia maksymalną ilość punktów zdrowia postaci.
        /// </summary>
        /// <value>
        /// Maksymalna ilość punktów zdrowia, która może mieć postać.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Wartość jest ograniczona od dołu przez 0.</para>
        /// <para>Zmiana wartości aktualizuje również bieżące zdrowie, jeśli przekracza nowe maksimum.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double MaximalHealth
        {
            get => _maximalHealth.Value(this, "MaximalHealth");
            protected set => _maximalHealth.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości CurrentHealth.
        /// Przechowuje aktualną wartość zdrowia postaci.
        /// </summary>
        public double _currentHealth;   
        
        /// <summary>
        /// Pobiera lub ustawia aktualną ilość punktów zdrowia postaci.
        /// </summary>
        /// <value>
        /// Aktualna ilość punktów zdrowia postaci.
        /// Wartość jest automatycznie ograniczana do zakresu od 0 do <see cref="MaximalHealth"/>.
        /// </value>
        /// <remarks>
        /// <para>Ustawienie wartości mniejszej niż 0 spowoduje ustawienie 0.</para>
        /// <para>Ustawienie wartości większej niż <see cref="MaximalHealth"/> spowoduje ustawienie <see cref="MaximalHealth"/>.</para>
        /// <para>Zmiana wartości może wywołać zdarzenia związane ze zmianą zdrowia.</para>
        /// </remarks>
        [JsonIgnore]
        public double CurrentHealth {
            get => _currentHealth;
            protected set => _currentHealth = Math.Clamp(value, 0, MaximalHealth);
        }
        /// <summary>
        /// Pole wspierające dla właściwości MinimalAttack.
        /// Przechowuje bazową wartość minimalnego ataku postaci.
        /// </summary>
        public Stat _minimalAttack;
        
        /// <summary>
        /// Pobiera lub ustawia minimalną wartość obrażeń zadawanych przez postać.
        /// </summary>
        /// <value>
        /// Minimalna wartość obrażeń, jaką może zadać postać w pojedynczym ataku.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Wartość ta określa dolną granicę przedziału obrażeń ataku podstawowego.</para>
        /// <para>Rzeczywiste obrażenia są losowane z przedziału [MinimalAttack, MaximalAttack].</para>
        /// <para>Wartość nie może przekroczyć wartości <see cref="MaximalAttack"/>.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double MinimalAttack {
            get => _minimalAttack.Value(this, "MinimalAttack");
            protected set => _minimalAttack.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości MaximalAttack.
        /// Przechowuje bazową wartość maksymalnego ataku postaci.
        /// </summary>
        public Stat _maximalAttack;
        
        /// <summary>
        /// Pobiera lub ustawia maksymalną wartość obrażeń zadawanych przez postać.
        /// </summary>
        /// <value>
        /// Maksymalna wartość obrażeń, jaką może zadać postać w pojedynczym ataku.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Wartość ta określa górną granicę przedziału obrażeń ataku podstawowego.</para>
        /// <para>Rzeczywiste obrażenia są losowane z przedziału [<see cref="MinimalAttack"/>, MaximalAttack].</para>
        /// <para>Podczas ustawiania wartości, jest ona automatycznie porównywana z wartością <see cref="MinimalAttack"/>
        /// i ustawiana jako większa z nich.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double MaximalAttack { 
            get  => _maximalAttack.Value(this, "MaximalAttack");
            protected set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        /// <summary>
        /// Pole wspierające dla właściwości Dodge.
        /// Przechowuje bazową wartość szansy na unik postaci.
        /// </summary>
        public Stat _dodge;
        
        /// <summary>
        /// Pobiera lub ustawia szansę na uniknięcie ataku przeciwnika.
        /// </summary>
        /// <value>
        /// Wartość z przedziału [0, 1] określająca szansę na uniknięcie ataku.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Wartość 0 oznacza brak szansy na unik, a 1 gwarantowany unik (100%).</para>
        /// <para>Rzeczywista szansa na unik może być modyfikowana przez różne efekty i umiejętności.</para>
        /// <para>Wartość jest ograniczona do przedziału [0, 1] podczas obliczeń.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double Dodge {
            get => _dodge.Value(this, "Dodge");
            protected set => _dodge.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości PhysicalDefense.
        /// Przechowuje bazową wartość obrony fizycznej postaci.
        /// </summary>
        public Stat _physicalDefense;
        
        /// <summary>
        /// Pobiera lub ustawia wartość obrony fizycznej postaci.
        /// </summary>
        /// <value>
        /// Wartość obrony fizycznej, która redukuje otrzymywane obrażenia fizyczne.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Im wyższa wartość obrony fizycznej, tym mniejsze obrażenia fizyczne otrzymuje postać.</para>
        /// <para>Redukcja obrażeń jest obliczana według wzoru: damage * damage / (damage + PhysicalDefense)</para>
        /// <para>Oznacza to, że każdy kolejny punkt obrony daje mniejszą korzyść.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double PhysicalDefense {
            get => _physicalDefense.Value(this, "PhysicalDefense");
            protected set => _physicalDefense.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości MagicDefense.
        /// Przechowuje bazową wartość obrony magicznej postaci.
        /// </summary>
        public Stat _magicDefense;
        
        /// <summary>
        /// Pobiera lub ustawia wartość obrony magicznej postaci.
        /// </summary>
        /// <value>
        /// Wartość obrony magicznej, która redukuje otrzymywane obrażenia magiczne.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Im wyższa wartość obrony magicznej, tym mniejsze obrażenia magiczne otrzymuje postać.</para>
        /// <para>Redukcja obrażeń jest obliczana według wzoru: damage * damage / (damage + MagicDefense)</para>
        /// <para>Oznacza to, że każdy kolejny punkt obrony magicznej daje mniejszą korzyść.</para>
        /// <para>W przeciwieństwie do obrony fizycznej, niektóre umiejętności mogą ignorować część lub całość obrony magicznej.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double MagicDefense
        {
            get => _magicDefense.Value(this, "MagicDefense");
            protected set => _magicDefense.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości CritChance.
        /// Przechowuje bazową wartość szansy na cios krytyczny.
        /// </summary>
        public Stat _critChance;
        
        /// <summary>
        /// Pobiera lub ustawia szansę na zadanie ciosu krytycznego.
        /// </summary>
        /// <value>
        /// Wartość z przedziału [0, 1] określająca szansę na cios krytyczny.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów i ograniczana do przedziału [0, 1].
        /// </value>
        /// <remarks>
        /// <para>Wartość 0 oznacza brak szansy na cios krytyczny, a 1 gwarantowany cios krytyczny (100%).</para>
        /// <para>Rzeczywista szansa na cios krytyczny może być modyfikowana przez różne efekty i umiejętności.</para>
        /// <para>Wartość jest automatycznie ograniczana do przedziału [0, 1] podczas pobierania.</para>
        /// <para>Cios krytyczny zwiększa zadawane obrażenia o wartość określoną przez <see cref="CritMod"/>.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double CritChance
        {
            get => Math.Clamp(_critChance.Value(this, "CritChance"), 0, 1);
            protected set => _critChance.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości Speed.
        /// Przechowuje bazową wartość szybkości postaci.
        /// </summary>
        public Stat _speed;
        
        /// <summary>
        /// Pobiera lub ustawia wartość szybkości postaci.
        /// </summary>
        /// <value>
        /// Wartość określająca szybkość postaci w walce.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów i dodatkowych efektów.
        /// </value>
        /// <remarks>
        /// <para>Im wyższa wartość szybkości, tym szybciej postać otrzymuje tury w walce.</para>
        /// <para>Jeśli typem zasobu postaci jest <see cref="ResourceType.Momentum">Momentum</see>,
        /// szybkość jest zwiększana o 10% aktualnej wartości zasobu.</para>
        /// <para>Szybkość ma kluczowe znaczenie dla kolejności wykonywania akcji w walce.</para>
        /// </remarks>
        [JsonIgnore]
        public double Speed
        {
            get => _speed.Value(this, "Speed") + (ResourceType == ResourceType.Momentum ? CurrentResource / 10 : 0);
            protected set => _speed.BaseValue = value;
        }

        /// <summary>
        /// Pole wspierające dla właściwości Accuracy.
        /// Przechowuje bazową wartość celności postaci.
        /// </summary>
        public Stat _accuracy; 
        
        /// <summary>
        /// Pobiera lub ustawia wartość celności postaci.
        /// </summary>
        /// <value>
        /// Wartość określająca szansę na trafienie przeciwnika.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów i dodatkowych efektów.
        /// </value>
        /// <remarks>
        /// <para>Im wyższa wartość celności, tym większa szansa na trafienie przeciwnika.</para>
        /// <para>Jeśli typem zasobu postaci jest <see cref="ResourceType.Fury">Furia</see>,
        /// celność jest zmniejszana o 1/3 aktualnej wartości zasobu.</para>
        /// <para>Rzeczywista szansa na trafienie jest porównywana z wartością uniku przeciwnika.</para>
        /// <para>Wartość ujemna oznacza karę do trafienia.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double Accuracy
        {
            get => _accuracy.Value(this, "Accuracy") - (ResourceType == ResourceType.Fury ? CurrentResource / 3 : 0);
            protected set => _accuracy.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości CritMod.
        /// Przechowuje bazową wartość mnożnika obrażeń krytycznych.
        /// </summary>
        public Stat _critMod; 
        
        /// <summary>
        /// Pobiera lub ustawia wartość mnożnika obrażeń krytycznych.
        /// </summary>
        /// <value>
        /// Wartość określająca, o ile zwiększają się obrażenia przy ciosie krytycznym.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Wartość 0.5 oznacza, że cios krytyczny zada 50% więcej obrażeń (1.5x).</para>
        /// <para>Wartość 1.0 oznacza, że cios krytyczny zada 100% więcej obrażeń (2.0x).</para>
        /// <para>Mnożnik jest dodawany do bazowych obrażeń, więc wartość 0.0 oznacza brak dodatkowych obrażeń krytycznych.</para>
        /// <para>Rzeczywisty wzrost obrażeń może być modyfikowany przez różne efekty i umiejętności.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double CritMod {
            get => _critMod.Value(this, "CritMod");
            protected set => _critMod.BaseValue = value;
        }
        /// <summary>
        /// Pole wspierające dla właściwości MaximalResource.
        /// Przechowuje bazową wartość maksymalnej ilości zasobu postaci.
        /// </summary>
        public Stat _maximalResource;
        
        /// <summary>
        /// Pobiera lub ustawia maksymalną ilość zasobu postaci.
        /// </summary>
        /// <value>
        /// Maksymalna wartość zasobu postaci, jaką może posiadać postać.
        /// Wartość jest obliczana z uwzględnieniem modyfikatorów.
        /// </value>
        /// <remarks>
        /// <para>Określa górny limit wartości <see cref="CurrentResource">CurrentResource</see>.</para>
        /// <para>Typ zasobu jest określony przez właściwość <see cref="ResourceType">ResourceType</see>.</para>
        /// <para>Różne klasy postaci mogą używać tej właściwości do określania maksymalnej wartości specyficznych zasobów, takich jak mana, energia itp.</para>
        /// </remarks>
        [JsonIgnore]
        public virtual double MaximalResource {
            get => _maximalResource.Value(this, "MaximalResource");
            protected set => _maximalResource.BaseValue = value;
        }
        
        /// <summary>
        /// Pole wspierające dla właściwości CurrentResource.
        /// Przechowuje aktualną wartość zasobu postaci.
        /// </summary>
        protected double _currentResource;

        /// <summary>
        /// Pobiera lub ustawia aktualną wartość zasobu postaci.
        /// </summary>
        /// <value>
        /// Aktualna wartość zasobu postaci, ograniczona do zakresu [0, <see cref="MaximalResource">MaximalResource</see>].
        /// </value>
        /// <remarks>
        /// <para>Wartość jest automatycznie ograniczana do przedziału [0, MaximalResource].</para>
        /// <para>Zmiana wartości wywołuje zdarzenie PropertyChanged dla powiązań danych.</para>
        /// <para>Typ zasobu jest określony przez właściwość <see cref="ResourceType">ResourceType</see>.</para>
        /// <para>Różne klasy postaci mogą używać tej właściwości do przechowywania specyficznych zasobów, takich jak mana, energia itp.</para>
        /// </remarks>
        [JsonIgnore]
        public double CurrentResource {
            get => _currentResource;
            protected set => _currentResource = Math.Clamp(value, 0, MaximalResource);
        }
        /// <summary>
        /// Pole wspierające dla właściwości ResourceRegen.
        /// Przechowuje bazową wartość regeneracji zasobu.
        /// </summary>
        public Stat _resourceRegen;
        
        /// <summary>
        /// Pobiera wartość regeneracji zasobu w danej turze.
        /// </summary>
        /// <value>
        /// Wartość określająca, ile jednostek zasobu zostanie przywróconych postaci w danej turze.
        /// </value>
        /// <remarks>
        /// <para>Dla typu zasobu <see cref="ResourceType.Momentum">Momentum</see>:
        /// - Jeśli szybkość (Speed) jest większa lub równa 100, zwraca Speed / 10
        /// - W przeciwnym razie zwraca Speed / 5</para>
        /// <para>Dla innych typów zasobów zwraca wartość z modyfikatorami.</para>
        /// <para>Wartość jest obliczana dynamicznie przy każdym wywołaniu, z uwzględnieniem aktualnego stanu postaci.</para>
        /// </remarks>
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

        /// <summary>
        /// Pobiera lub ustawia typ zasobu używany przez postać.
        /// </summary>
        /// <value>
        /// Typ zasobu określający, jakiego rodzaju zasób wykorzystuje postać (np. mana, energia, furia itp.).
        /// </value>
        /// <remarks>
        /// <para>Typ zasobu wpływa na sposób działania właściwości <see cref="CurrentResource">CurrentResource</see> i <see cref="ResourceRegen">ResourceRegen</see>.</para>
        /// <para>Różne klasy postaci mogą mieć różne typy zasobów, które wpływają na ich umiejętności i styl gry.</para>
        /// </remarks>
        public ResourceType ResourceType { get; set; }
        
        /// <summary>
        /// Pobiera lub ustawia poziom postaci.
        /// </summary>
        /// <value>
        /// Poziom postaci, który wpływa na jej statystyki i dostępne umiejętności.
        /// </value>
        /// <remarks>
        /// <para>Wzrost poziomu zazwyczaj zwiększa podstawowe statystyki postaci.</para>
        /// <para>Poziom może również odblokowywać nowe umiejętności lub ulepszenia.</para>
        /// <para>Wartość powinna być nieujemna.</para>
        /// </remarks>
        public int Level { get; set; }

        /// <summary>
        /// Pobiera lub ustawia listę efektów pasywnych aktywnie działających na postać.
        /// </summary>
        /// <value>
        /// Kolekcja efektów pasywnych, które modyfikują statystyki lub zachowanie postaci.
        /// </value>
        /// <remarks>
        /// <para>Efekty pasywne mogą pochodzić z umiejętności, przedmiotów lub innych źródeł.</para>
        /// <para>Każdy efekt może modyfikować różne statystyki postaci.</para>
        /// <para>Efekty mogą mieć określony czas trwania lub być stałe.</para>
        /// </remarks>
        public PassiveEffectList PassiveEffects { get; set; }
        
        /// <summary>
        /// Pobiera lub ustawia słownik odporności na różne typy efektów statusowych.
        /// </summary>
        /// <value>
        /// Słownik, gdzie kluczem jest typ efektu statusowego, a wartością statystyka określająca odporność na ten efekt.
        /// </value>
        /// <remarks>
        /// <para>Im wyższa wartość odporności, tym mniejsza szansa na otrzymanie danego efektu statusowego.</para>
        /// <para>Odporność może być modyfikowana przez różne efekty i umiejętności.</para>
        /// <para>Wartość 0 oznacza brak odporności, a 1 całkowitą odporność (100%).</para>
        /// </remarks>
        public Dictionary<StatusEffectType, Stat> Resistances { get; set; }
        
        /// <summary>
        /// Pobiera lub ustawia tablicę aktywnych umiejętności dostępnych dla postaci.
        /// </summary>
        /// <value>
        /// Tablica obiektów <see cref="ActiveSkill"/> reprezentujących umiejętności, które postać może używać w walce.
        /// </value>
        /// <remarks>
        /// <para>Każda postać może posiadać określoną liczbę aktywnych umiejętności, które może używać podczas walki.</para>
        /// <para>Umiejętności mogą wymagać określonych warunków do użycia (np. odpowiedniej ilości zasobów).</para>
        /// <para>Dostępne umiejętności mogą się zmieniać w zależności od poziomu postaci i wyposażenia.</para>
        /// </remarks>
        public ActiveSkill[] ActiveSkills { get; set; }
        
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Character"/> z domyślnymi wartościami.
        /// </summary>
        /// <remarks>
        /// Konstruktor chroniony, który może być wywoływany tylko przez klasy dziedziczące.
        /// Inicjalizuje podstawowe właściwości postaci z domyślnymi wartościami.
        /// </remarks>
        protected Character() { }
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="Character"/> z określonymi parametrami.
        /// </summary>
        /// <param name="name">Nazwa postaci.</param>
        /// <param name="maxHealth">Statystyka maksymalnego zdrowia postaci.</param>
        /// <param name="minimalAttack">Minimalna wartość ataku.</param>
        /// <param name="maximalAttack">Maksymalna wartość ataku.</param>
        /// <param name="critChance">Szansa na cios krytyczny.</param>
        /// <param name="dodge">Szansa na unik.</param>
        /// <param name="physicalDefense">Obrona fizyczna.</param>
        /// <param name="magicDefense">Obrona magiczna.</param>
        /// <param name="speed">Szybkość postaci.</param>
        /// <param name="accuracy">Celność ataku.</param>
        /// <param name="critMod">Mnożnik obrażeń krytycznych.</param>
        /// <param name="level">Poziom postaci.</param>
        /// <remarks>
        /// Konstruktor chroniony, który może być wywoływany tylko przez klasy dziedziczące.
        /// Inicjalizuje wszystkie statystyki postaci na podstawie podanych parametrów.
        /// </remarks>
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
        /// <summary>
        /// Zadaje obrażenia postaci, uwzględniając tarcze i modyfikatory obrażeń.
        /// </summary>
        /// <param name="damageType">Typ zadawanych obrażeń.</param>
        /// <param name="damage">Podstawowa wartość obrażeń do zadania.</param>
        /// <param name="source">Źródło obrażeń (np. postać lub efekt).</param>
        /// <returns>Rzeczywista ilość obrażeń zadanych postaci po uwzględnieniu wszystkich modyfikatorów.</returns>
        /// <remarks>
        /// <para>Metoda uwzględnia szansę na absorpcję obrażeń magicznych.</para>
        /// <para>Wyświetla informację o otrzymanych obrażeniach w interfejsie użytkownika.</para>
        /// <para>Uwzględnia działanie tarcz, które mogą zredukować otrzymane obrażenia.</para>
        /// <para>Automatycznie aktualizuje wartość zdrowia postaci.</para>
        /// </remarks>
        public double TakeDamage(DamageType damageType, double damage, dynamic source)
        {
            if (damageType == DamageType.Magic &&
                Random.Shared.NextDouble() < UtilityMethods.CalculateModValue(0, 
                    PassiveEffects.GetModifiers("AbsorptionChance"))) return 0; //TODO: "DAMAGE ABSORBED!" or sth
            double damageTaken = DamageMitigated(damage, damageType, source);
            var shields = PassiveEffects.TimedEffects.Where(e => e.Type == "Shield").ToList();
            if (shields.Count > 0)
                damageTaken = StatusEffectHandler.TakeShieldsDamage(shields, this, damageTaken);
            CurrentHealth -= damageTaken;
            return damageTaken;
        }

        /// <summary>
        /// Zużywa określoną ilość zasobu postaci.
        /// </summary>
        /// <param name="amount">Ilość zasobu do zużycia.</param>
        /// <remarks>
        /// <para>Metoda zmniejsza ilość aktualnego zasobu o podaną wartość.</para>
        /// <para>W przypadku zasobu innego niż Fury (Wściekłość), wartość zasobu nie może spaść poniżej zera.</para>
        /// <para>Dla zasobu typu Fury, wartość może być ujemna (możliwe jest wejście na "debet").</para>
        /// <para>Automatycznie aktualizuje wartość zasobu w interfejsie użytkownika.</para>
        /// </remarks>
        public void UseResource(int amount)
        {
            CurrentResource -= amount;
            if (ResourceType != ResourceType.Fury)
                CurrentResource = Math.Max(CurrentResource, 0);
        }

        /// <summary>
        /// Pobiera obiekt statystyki na podstawie jej nazwy.
        /// </summary>
        /// <param name="stat">Nazwa statystyki do pobrania.</param>
        /// <returns>Obiekt <see cref="Stat"/> reprezentujący żądaną statystykę lub null, jeśli statystyka o podanej nazwie nie istnieje.</returns>
        /// <remarks>
        /// <para>Metoda obsługuje następujące nazwy statystyk:</para>
        /// <list type="bullet">
        /// <item>MaximalHealth - Maksymalne zdrowie</item>
        /// <item>MinimalAttack - Minimalny atak</item>
        /// <item>MaximalAttack - Maksymalny atak</item>
        /// <item>CritChance - Szansa na cios krytyczny</item>
        /// <item>Dodge - Szansa na unik</item>
        /// <item>PhysicalDefense - Obrona fizyczna</item>
        /// <item>MagicDefense - Obrona magiczna</item>
        /// <item>ResourceRegen - Regeneracja zasobu</item>
        /// <item>Speed - Szybkość</item>
        /// <item>Accuracy - Celność</item>
        /// <item>CritMod - Mnożnik obrażeń krytycznych</item>
        /// <item>MaximalResource - Maksymalny zasób</item>
        /// </list>
        /// <para>Wartość zwracana może być używana do modyfikowania statystyk postaci.</para>
        /// </remarks>
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

        /// <summary>
        /// Regeneruje zasób postaci o określoną wartość, uwzględniając modyfikatory.
        /// </summary>
        /// <param name="amount">Podstawowa ilość zasobu do zregenerowania.</param>
        /// <remarks>
        /// <para>Metoda zwiększa ilość aktualnego zasobu o podaną wartość, uwzględniając modyfikatory regeneracji.</para>
        /// <para>Nie działa, jeśli postać ma efekt "NoResourceRegen".</para>
        /// <para>Uwzględnia modyfikatory regeneracji zasobu (ResourceRegenMod).</para>
        /// <para>Automatycznie dba o to, aby wartość zasobu nie przekroczyła wartości maksymalnej.</para>
        /// <para>Wartość zasobu jest zaokrąglana do najbliższej liczby całkowitej.</para>
        /// </remarks>
        public void RegenResource(int amount)
        {
            if (PassiveEffects.InnateEffects.Any(x => x.Type == "NoResourceRegen")) return;
            CurrentResource = Math.Min(CurrentResource + UtilityMethods.
                CalculateModValue(amount, PassiveEffects.GetModifiers("ResourceRegenMod")), MaximalResource);
            //CharacterEventTextService.DisplayResourceRegenText(this, amount);
        }

        /// <summary>
        /// Oblicza rzeczywistą wartość obrażeń po uwzględnieniu wszystkich mechanik obronnych.
        /// </summary>
        /// <param name="damage">Podstawowa wartość obrażeń.</param>
        /// <param name="damageType">Typ zadawanych obrażeń.</param>
        /// <param name="source">Źródło obrażeń (np. postać lub efekt).</param>
        /// <returns>Rzeczywista wartość obrażeń po uwzględnieniu wszystkich modyfikatorów.</returns>
        /// <remarks>
        /// <para>Metoda uwzględnia następujące mechaniki obronne:</para>
        /// <list type="bullet">
        /// <item>Penetrację pancerza (fizycznego i magicznego) źródła obrażeń</item>
        /// <item>Redukcję obrażeń w zależności od typu obrażeń i obrony</item>
        /// <item>Odporności na efekty statusowe (np. krwawienie, trucizna, podpalenie)</item>
        /// <item>Modyfikatory obrażeń (ogólne i specyficzne dla typu obrażeń)</item>
        /// </list>
        /// <para>Minimalna wartość zwracanych obrażeń to 1.</para>
        /// <para>Dla każdego typu obrażeń stosowane są odpowiednie modyfikatory obrażeń.</para>
        /// </remarks>
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
        /// <summary>
        /// Leczy postać o określoną wartość zdrowia.
        /// </summary>
        /// <param name="heal">Ilość punktów zdrowia do przywrócenia.</param>
        /// <remarks>
        /// <para>Metoda zwiększa wartość zdrowia postaci o podaną wartość.</para>
        /// <para>Automatycznie dba o to, aby wartość zdrowia nie przekroczyła wartości maksymalnej.</para>
        /// <para>Wyświetla informację o uleczonych punktach zdrowia w interfejsie użytkownika.</para>
        /// <para>Nie ma efektu, jeśli postać ma pełne zdrowie.</para>
        /// <para>Ujemna wartość parametru spowoduje zmniejszenie zdrowia postaci.</para>
        /// </remarks>
        public void Heal(double heal) {
            CurrentHealth += heal;
        }

        /// <summary>
        /// Dodaje modyfikator do określonej statystyki postaci.
        /// </summary>
        /// <param name="stat">Typ statystyki, która ma zostać zmodyfikowana.</param>
        /// <param name="modifier">Modyfikator do dodania.</param>
        /// <exception cref="ArgumentOutOfRangeException">Wyrzucany, gdy podany typ statystyki jest nieprawidłowy.</exception>
        /// <remarks>
        /// <para>Metoda dodaje modyfikator do wybranej statystyki postaci.</para>
        /// <para>Obsługiwane typy statystyk to:</para>
        /// <list type="bullet">
        /// <item>MaximalHealth - Maksymalne zdrowie</item>
        /// <item>MinimalAttack - Minimalny atak</item>
        /// <item>MaximalAttack - Maksymalny atak</item>
        /// <item>Dodge - Szansa na unik</item>
        /// <item>PhysicalDefense - Obrona fizyczna</item>
        /// <item>MagicDefense - Obrona magiczna</item>
        /// <item>CritChance - Szansa na cios krytyczny</item>
        /// <item>Speed - Szybkość</item>
        /// <item>Accuracy - Celność</item>
        /// <item>MaximalResource - Maksymalny zasób</item>
        /// </list>
        /// <para>Modyfikator może zwiększać, zmniejszać lub ustawiać nową wartość statystyki.</para>
        /// </remarks>
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

        /// <summary>
        /// Dodaje modyfikator do odporności na określony efekt statusowy.
        /// </summary>
        /// <param name="stat">Typ efektu statusowego, którego odporność ma zostać zmodyfikowana.</param>
        /// <param name="modifier">Modyfikator do dodania.</param>
        /// <remarks>
        /// <para>Metoda dodaje modyfikator do odporności na wybrany efekt statusowy.</para>
        /// <para>Odporność określa szansę na uniknięcie efektu statusowego (np. ogłuszenie, podpalenie).</para>
        /// <para>Im wyższa wartość odporności, tym mniejsza szansa na otrzymanie danego efektu.</para>
        /// <para>Modyfikator może zwiększać, zmniejszać lub ustawiać nową wartość odporności.</para>
        /// <para>Wartość 0 oznacza brak odporności, a 1 całkowitą odporność (100%).</para>
        /// </remarks>
        public void AddResistanceModifier(StatusEffectType stat, StatModifier modifier)
        {
            Resistances[stat].AddModifier(modifier);
        }
        

        /// <summary>
        /// Aktualizuje wszystkie modyfikatory statystyk postaci.
        /// </summary>
        /// <remarks>
        /// <para>Metoda wywołuje metodę Tick() dla wszystkich statystyk postaci, co powoduje:</para>
        /// <list type="bullet">
        /// <item>Zmniejszenie czasu trwania modyfikatorów czasowych</item>
        /// <item>Usunięcie wygasłych modyfikatorów</item>
        /// <li>Przeliczenie wartości statystyk z uwzględnieniem aktualnych modyfikatorów</li>
        /// </list>
        /// <para>Metoda powinna być wywoływana w każdej turze walki lub w odpowiednich momentach gry.</para>
        /// <para>Obsługiwane statystyki to: zdrowie, atak, obrona, unik, szybkość i inne.</para>
        /// </remarks>
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

        /// <summary>
        /// Pobiera słownik wszystkich aktywnych modyfikatorów statystyk postaci.
        /// </summary>
        /// <returns>Słownik, gdzie kluczem jest modyfikator, a wartością typ statystyki, której dotyczy.</returns>
        /// <remarks>
        /// <para>Metoda zwraca kompletną listę wszystkich aktywnych modyfikatorów przypisanych do statystyk postaci.</para>
        /// <para>Każdy wpis w słowniku zawiera:
        /// <list type="bullet">
        /// <item>Klucz: Obiekt modyfikatora (StatModifier)</item>
        /// <item>Wartość: Typ statystyki (StatType), której dotyczy modyfikator</item>
        /// </list>
        /// </para>
        /// <para>Przydatne do wyświetlania informacji o wszystkich aktywnych modyfikatorach w interfejsie użytkownika.</para>
        /// <para>Zwracane modyfikatory obejmują wszystkie statystyki postaci, w tym zdrowie, atak, obronę itp.</para>
        /// </remarks>
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