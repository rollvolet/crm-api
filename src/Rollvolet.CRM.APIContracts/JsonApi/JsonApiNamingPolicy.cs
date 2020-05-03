using System.Text;
using System.Text.Json;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public static class JsonApiStringExtensions
    {
        // copied from https://stackoverflow.com/a/54012346
        public static string ToKebabCase(this string value)
        {
            if (value is null)
                return null;

            if (value.Length == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();

            for(var i = 0; i < value.Length; i++)
            {
                if (char.IsLower(value[i])) // if current char is already lowercase
                {
                    builder.Append(value[i]);
                }
                else if(i == 0) // if current char is the first char
                {
                    builder.Append(char.ToLower(value[i]));
                }
                else if (char.IsLower(value[i - 1])) // if current char is upper and previous char is lower
                {
                    builder.Append("-");
                    builder.Append(char.ToLower(value[i]));
                }
                else if(i + 1 == value.Length || char.IsUpper(value[i + 1])) // if current char is upper and next char doesn't exist or is upper
                {
                    builder.Append(char.ToLower(value[i]));
                }
                else // if current char is upper and next char is lower
                {
                    builder.Append("-");
                    builder.Append(char.ToLower(value[i]));
                }
            }
            return builder.ToString();
        }
    }

    public class JsonApiNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToKebabCase();
    }
}