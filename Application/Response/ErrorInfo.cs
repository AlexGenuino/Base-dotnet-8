using System.Text.Json;

namespace Application.Response
{
    public record ErrorInfo
    {
        public string? ErrorDescription { get; set; }
        public int HttpStatus { get; set; }

        public string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            return JsonSerializer.Serialize(this, options);
        }

        public ErrorInfo(string error)
        {
            HttpStatus = 400;
            ErrorDescription = error;
        }

        public ErrorInfo(string error, int statusCode)
        {
            HttpStatus = statusCode;
            ErrorDescription = error;
        }
    }
}
