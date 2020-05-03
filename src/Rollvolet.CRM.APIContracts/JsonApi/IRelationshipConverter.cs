using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class IRelationshipConverter : JsonConverter<IRelationship>
    {

        public override bool CanConvert(Type typeToConvert) => typeof(IRelationship).IsAssignableFrom(typeToConvert);

        public override IRelationship Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new InvalidOperationException();
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