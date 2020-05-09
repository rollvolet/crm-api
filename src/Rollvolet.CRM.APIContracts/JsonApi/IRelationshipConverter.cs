using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class IRelationshipConverter : JsonConverter<IRelationship>
    {

        public override bool CanConvert(Type typeToConvert) => typeof(IRelationship).IsAssignableFrom(typeToConvert);

        public override IRelationship Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IRelationship relationship = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                // incoming JSON-string is not an object. No idea how to deserialize to an IRelationship object in that case.
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    // we reached the end of the relationship JSON-string. Let's return the deserialized object.
                    return relationship;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    if (propertyName == "data")
                    {
                        if (typeToConvert == typeof(OneRelationship))
                        {
                            var data = JsonSerializer.Deserialize<RelatedResource>(ref reader, options);
                            relationship = new OneRelationship { Data = data };
                        }
                        else if (typeToConvert == typeof(ManyRelationship))
                        {
                            var data = JsonSerializer.Deserialize<IEnumerable<RelatedResource>>(ref reader, options);
                            relationship = new ManyRelationship { Data = data };
                        }
                        else
                        {
                            // Is there an implementation of the IRelationship interface not listed here?
                            throw new JsonException();
                        }
                    }
                    else
                    {
                        // we're only interested in the 'data' property of a relationship for incoming requests
                        reader.Skip();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRelationship relationship, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Links property
            writer.WritePropertyName("links");
            writer.WriteStartObject();
            writer.WriteString("self", relationship.Links.Self);
            writer.WriteString("related", relationship.Links.Related);
            writer.WriteEndObject();

            // Data property for included resources
            if (relationship is ManyRelationship manyRelationship)
            {
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, manyRelationship.Data, options);
            }
            else if (relationship is OneRelationship oneRelationship)
            {
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, oneRelationship.Data, options);
            }

            writer.WriteEndObject();
        }
    }
}