using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Drops;

/// <summary>
/// Klasa reprezentująca pulę przedmiotów, z których może coś wypaść.
/// Zawiera słownik przedmiotów i ich parametrów oraz tablicę szans na wypadnięcie.
/// </summary>
public class DropPool
{
    /// <summary>
    /// Słownik zawierający przedmioty w puli, gdzie kluczem jest alias przedmiotu,
    /// a wartością obiekt ItemDrop określający parametry wypadnięcia.
    /// </summary>
    public Dictionary<string, ItemDrop> Pool { get; set; }
    
    /// <summary>
    /// Tablica szans na wypadnięcie przedmiotu z puli.
    /// Każdy element tablicy określa szansę na dodatkowy przedmiot z puli.
    /// </summary>
    public double[] Chances { get; set; }

    /// <summary>
    /// Konstruktor domyślny wymagany do deserializacji JSON.
    /// </summary>
    public DropPool() { }

    /// <summary>
    /// Konstruktor kopiujący tworzący głęboką kopię obiektu DropPool.
    /// </summary>
    /// <param name="other">Obiekt do skopiowania.</param>
    public DropPool(DropPool other)
    {
        Pool = other.Pool.ToDictionary(x => x.Key, x => 
            new ItemDrop(x.Value.MinLevel, x.Value.MaxLevel, x.Value.Weight, x.Value.MinAmount, x.Value.MaxAmount));
        Chances = (double[])other.Chances.Clone();
    }

    /// <summary>
    /// Wybiera losowy przedmiot z puli, uwzględniając poziom postaci.
    /// </summary>
    /// <param name="level">Poziom postaci, dla którego wybierany jest przedmiot.</param>
    /// <returns>Para zawierająca przedmiot i jego parametry.</returns>
    public KeyValuePair<IItem, ItemDrop> Choice(int level)
    {
        var choice = UtilityMethods.RandomChoice(Pool
            .Where(item => item.Value.MinLevel <= level && item.Value.MaxLevel >= level)
            .ToDictionary(item => item.Key, item => item.Value)
            .ToDictionary(x => x, x => x.Value.Weight));
        var item = new KeyValuePair<IItem, ItemDrop>();
        var choiceSplit = choice.Key.Split('_');
        item = choiceSplit[0] switch
        {
            "WeaponDrop" => new KeyValuePair<IItem, ItemDrop>(EquippableItemService.GetRandomWeapon(level / 10 + 1),
                choice.Value),
            "ArmorDrop" => new KeyValuePair<IItem, ItemDrop>(EquippableItemService.GetRandomArmor(level / 10 + 1),
                choice.Value),
            "BossEquipmentDrop" => new KeyValuePair<IItem, ItemDrop>(
                EquippableItemService.GetBossDrop(level / 10 + 1, choiceSplit[1]), choice.Value),
            "GalduriteDrop" => new KeyValuePair<IItem, ItemDrop>(
                new Galdurite(Random.Shared.Next(0, 2) == 1, level < 21 ? 1 : level < 41 ? 2 : 3,
                    Convert.ToInt32(choiceSplit[1])), choice.Value),
            _ => new KeyValuePair<IItem, ItemDrop>(ItemManager.GetItem(choice.Key), choice.Value)
        };
        return item;
    }

    /// <summary>
    /// Pobiera losowy przedmiot z puli z losową ilością.
    /// </summary>
    /// <param name="level">Poziom postaci, dla którego wybierany jest przedmiot.</param>
    /// <returns>Para zawierająca przedmiot i losową ilość z zakresu zdefiniowanego w ItemDrop.</returns>
    public KeyValuePair<IItem, int> GetDrop(int level)
    {
        var drop = Choice(level);
        return new KeyValuePair<IItem, int>(drop.Key, 
            Random.Shared.Next(drop.Value.MinAmount, drop.Value.MaxAmount + 1));
    }
}