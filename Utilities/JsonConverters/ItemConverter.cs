using GodmistWPF.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GodmistWPF.Utilities.JsonConverters;

/// <summary>
/// Konwerter JSON do serializacji i deserializacji słownika przedmiotów i ich ilości.
/// Obsługuje różne typy przedmiotów implementujących interfejs IItem.
/// </summary>
public class ItemConverter : JsonConverter<Dictionary<IItem, int>>
{
    /// <summary>
    /// Serializuje słownik przedmiotów i ich ilości do formatu JSON.
    /// </summary>
    /// <param name="writer">Pisarz JSON.</param>
    /// <param name="value">Słownik zawierający pary przedmiot-ilość do zserializowania.</param>
    /// <param name="serializer">Wystąpienie serializatora JSON.</param>
    public override void WriteJson(JsonWriter writer, Dictionary<IItem, int> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        int index = 0; // To ensure unique keys even if items are of the same type
        foreach (var kvp in value)
        {
            // Create a unique key by combining type name and index
            var uniqueKey = $"{kvp.Key.GetType().Name}_{index++}";

            writer.WritePropertyName(uniqueKey);

            // Start an object to group the item and quantity
            writer.WriteStartObject();
        
            // Serialize the item
            writer.WritePropertyName("Item");
            serializer.Serialize(writer, kvp.Key);

            // Serialize the quantity
            writer.WritePropertyName("Quantity");
            writer.WriteValue(kvp.Value);

            writer.WriteEndObject();
        }
        writer.WriteEndObject();
    }

    /// <summary>
    /// Deserializuje obiekt JSON do słownika przedmiotów i ich ilości.
    /// </summary>
    /// <param name="reader">Czytnik JSON.</param>
    /// <param name="objectType">Typ obiektu do przekonwertowania.</param>
    /// <param name="existingValue">Istniejąca wartość obiektu.</param>
    /// <param name="hasExistingValue">Czy istnieje wartość dla obiektu docelowego.</param>
    /// <param name="serializer">Wystąpienie serializatora JSON.</param>
    /// <returns>Zdeserializowany słownik przedmiotów i ich ilości.</returns>
    public override Dictionary<IItem, int> ReadJson(JsonReader reader, Type objectType, Dictionary<IItem, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var items = new Dictionary<IItem, int>();
        var jsonObject = JObject.Load(reader);

        foreach (var property in jsonObject.Properties())
        {
            // Extract the item type name from the unique key (e.g., "Sword_0" -> "Sword")
            var itemTypeName = property.Name.Split('_')[0];
            var itemType = Type.GetType($"ConsoleGodmist.Items.{itemTypeName}");
            if (itemType == null) continue;

            // The value of the property is an object containing "Item" and "Quantity"
            var itemObject = (JObject)property.Value;

            // Deserialize the item
            var item = (IItem)itemObject["Item"].ToObject(itemType, serializer);

            // Deserialize the quantity
            var quantity = itemObject["Quantity"].ToObject<int>();

            items.Add(item, quantity);
        }
    
        return items;
    }
}