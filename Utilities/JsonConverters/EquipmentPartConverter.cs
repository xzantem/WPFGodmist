using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GodmistWPF.Utilities.JsonConverters;

/// <summary>
/// Konwerter JSON do serializacji i deserializacji obiektów implementujących interfejs IEquipmentPart.
/// Obsługuje różne typy części wyposażenia i zapisuje ich typ w pliku JSON.
/// </summary>
public class EquipmentPartConverter : JsonConverter
{
    /// <summary>
    /// Określa, czy konwerter może obsłużyć określony typ.
    /// </summary>
    /// <param name="objectType">Typ obiektu do sprawdzenia.</param>
    /// <returns>True, jeśli konwerter może obsłużyć dany typ; w przeciwnym razie false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEquipmentPart).IsAssignableFrom(objectType);
    }

    /// <summary>
    /// Deserializuje obiekt JSON do odpowiedniego typu części wyposażenia.
    /// </summary>
    /// <param name="reader">Czytnik JSON.</param>
    /// <param name="objectType">Typ obiektu do przekonwertowania.</param>
    /// <param name="existingValue">Istniejąca wartość obiektu.</param>
    /// <param name="serializer">Wystąpienie serializatora JSON.</param>
    /// <returns>Zdeserializowany obiekt części wyposażenia.</returns>
    /// <exception cref="NotSupportedException">Wyrzucany, gdy typ części wyposażenia nie jest obsługiwany.</exception>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["Type"]?.ToString();

        IEquipmentPart result = type switch
        {
            "WeaponHead" => new WeaponHead(),
            "WeaponBinder" => new WeaponBinder(),
            "WeaponHandle" => new WeaponHandle(),
            "ArmorPlate" => new ArmorPlate(),
            "ArmorBinder" => new ArmorBinder(),
            "ArmorBase" => new ArmorBase(),
            _ => throw new NotSupportedException($"Unknown type: {type}")
        };

        serializer.Populate(jsonObject.CreateReader(), result);
        return result;
    }

    /// <summary>
    /// Serializuje obiekt części wyposażenia do formatu JSON.
    /// </summary>
    /// <param name="writer">Pisarz JSON.</param>
    /// <param name="value">Wartość do zserializowania.</param>
    /// <param name="serializer">Wystąpienie serializatora JSON.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var jsonObject = new JObject
        {
            ["Type"] = value.GetType().Name
        };

        // Serialize the object's properties directly to the JObject.
        foreach (var property in value.GetType().GetProperties())
        {
            // Add each property to the JObject.
            if (property.CanRead)
            {
                var propertyValue = property.GetValue(value);
                jsonObject[property.Name] = propertyValue != null ? JToken.FromObject(propertyValue, serializer) : JValue.CreateNull();
            }
        }

        // Write the resulting JObject to the writer.
        jsonObject.WriteTo(writer);
    }
}