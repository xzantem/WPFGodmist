using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GodmistWPF.Utilities.JsonConverters;

public class EquipmentPartConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEquipmentPart).IsAssignableFrom(objectType);
    }

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