using GodmistWPF.Combat.Skills;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GodmistWPF.Utilities.JsonConverters;

public class ActiveSkillEffectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IActiveSkillEffect).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["Type"]?.ToString();

        IActiveSkillEffect result = type switch
        {
            "AdvanceMove" => new AdvanceMove(),
            "BuffStat" => new BuffStat(),
            "ClearStatusEffect" => new ClearStatusEffect(),
            "DealDamage" => new DealDamage(),
            "DebuffResistance" => new DebuffResistance(),
            "DebuffStat" => new DebuffStat(),
            "ExtendDoT" => new ExtendDoT(),
            "GainShield" => new GainShield(),
            "HealTarget" => new HealTarget(),
            "InflictDoTStatusEffect" => new InflictDoTStatusEffect(),
            "InflictGenericStatusEffect" => new InflictGenericStatusEffect(),
            "InflictTimedPassiveEffect" => new InflictTimedPassiveEffect(),
            "RegenResource" => new RegenResource(),
            "ToggleInnatePassiveEffect" => new ToggleInnatePassiveEffect(),
            "ToggleListenerPassiveEffect" => new ToggleListenerPassiveEffect(),
            "TradeHealthForResource" => new TradeHealthForResource(),  
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