using System.Text.Json.Serialization;

namespace FlowerShop.Utility.Models
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("@odata.context")]
        public string? Context { get; set; }

        [JsonPropertyName("value")]
        public T? Value { get; set; }
    }
}
