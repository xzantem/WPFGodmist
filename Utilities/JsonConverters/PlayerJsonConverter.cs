using System.Reflection;
using GodmistWPF.Characters.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GodmistWPF.Utilities.JsonConverters;

/// <summary>
/// Konwerter JSON do serializacji i deserializacji obiektów klasy PlayerCharacter i jej pochodnych.
/// Obsługuje różne klasy postaci gracza i zapisuje ich typ w pliku JSON.
/// </summary>
public class PlayerJsonConverter : JsonConverter
{
    /// <summary>
    /// Określa, czy konwerter może obsłużyć określony typ.
    /// </summary>
    /// <param name="objectType">Typ obiektu do sprawdzenia.</param>
    /// <returns>True, jeśli konwerter może obsłużyć dany typ; w przeciwnym razie false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(PlayerCharacter).IsAssignableFrom(objectType);
    }

        /// <summary>
        /// Deserializuje obiekt JSON do odpowiedniej klasy postaci gracza.
        /// </summary>
        /// <param name="reader">Czytnik JSON.</param>
        /// <param name="objectType">Typ obiektu do przekonwertowania.</param>
        /// <param name="existingValue">Istniejąca wartość obiektu.</param>
        /// <param name="serializer">Wystąpienie serializatora JSON.</param>
        /// <returns>Zdeserializowany obiekt postaci gracza.</returns>
        /// <exception cref="NotSupportedException">Wyrzucany, gdy klasa postaci nie jest obsługiwana.</exception>
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

        /// <summary>
        /// Serializuje obiekt postaci gracza do formatu JSON.
        /// Zapisuje wszystkie pola publiczne i niepubliczne oraz właściwości, które nie są oznaczone atrybutem JsonIgnore.
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