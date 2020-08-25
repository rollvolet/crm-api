using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
  // Information about timezone is not stored together with a datetime in an SQL DB
  // As a consequence, all datetimes retrieved through queries have DateTimeKind.Unspecified
  // and are exposed without timezone information in the JSONAPI
  // E.g. "2019-12-09T00:00:00" instead of "2019-12-09T00:00:00Z"
  // The frontend wrongly interprets the datetime in the user's local timezone.
  // Using a custom converter we will make interpret all datetimes as UTC time during serialization
  // See also: https://stackoverflow.com/questions/58102189/formatting-datetime-in-asp-net-core-3-0-using-system-text-json
  public class DateTimeConverter : JsonConverter<DateTime>
  {
      public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
          Debug.Assert(typeToConvert == typeof(DateTime));
          var dateTimeString = reader.GetString();
          // DateTime.Parse() seems to ignore the 'Z' timezone specification and interprets the datetime as local time
          // DateTimeOffset.Parse() takes the timezone into account
          var localDateTime = DateTimeOffset.Parse(dateTimeString);
          return localDateTime.UtcDateTime;
      }

      public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
      {
          var utcDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
          writer.WriteStringValue(utcDateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
      }
  }
}