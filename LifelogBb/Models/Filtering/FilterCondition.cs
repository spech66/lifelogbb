using System.Text.Json.Serialization;

namespace LifelogBb.Models.Filtering
{
    public class FilterCondition
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("operator")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComparisonOperator Operator { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
}
