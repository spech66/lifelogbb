using System.Text.Json.Serialization;

namespace LifelogBb.Models.Filtering
{
    public class FilterGroup
    {
        [JsonPropertyName("operator")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogicalOperator Operator { get; set; } = LogicalOperator.And;

        [JsonPropertyName("conditions")]
        public List<FilterCondition> Conditions { get; set; } = new();

        [JsonPropertyName("groups")]
        public List<FilterGroup> Groups { get; set; } = new();
    }
}
