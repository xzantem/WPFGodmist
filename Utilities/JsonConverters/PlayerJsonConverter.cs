using System.Reflection;
using GodmistWPF.Characters.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GodmistWPF.Utilities.JsonConverters;

public class PlayerJsonConverter : JsonConverter
{
        public override bool CanConvert(Type objectType)
        {
            return typeof(PlayerCharacter).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            //Console.WriteLine("JSON Object: " + jsonObject.ToString()); // Log JSON content

            var characterClass = jsonObject["CharacterClass"]?.ToObject<int>();

            PlayerCharacter result = characterClass switch
            {
                0 => new Warrior(),
                // Add cases for other classes here
                1 => new Scout(),
                2 => new Sorcerer(),
                3 => new Paladin(),
                _ => throw new NotSupportedException($"Unknown class: {characterClass}")
            };

            // Populate the resulting object with the JSON properties.
            serializer.Populate(jsonObject.CreateReader(), result);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsonObject = new JObject
            {
                ["Type"] = value.GetType().Name
            };
            
            foreach (var field in value.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(value);
                jsonObject[field.Name] = fieldValue != null ? JToken.FromObject(fieldValue, serializer) : JValue.CreateNull();
            }

            // Serialize only the properties that are not marked with JsonIgnore.
            foreach (var property in value.GetType().GetProperties())
            {
                if (property.CanRead && !Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                {
                    var propertyValue = property.GetValue(value);
                    jsonObject[property.Name] = propertyValue != null ? JToken.FromObject(propertyValue, serializer) : JValue.CreateNull();
                }
            }

            jsonObject.WriteTo(writer);
        }
    }