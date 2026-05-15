using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Views.Shared.Components.FilterBuilder
{
    public class FilterBuilder : ViewComponent
    {
        public IViewComponentResult Invoke(Type entityType, string? currentFilter = null)
        {
            var filters = BuildFilterConfig(entityType);
            ViewBag.FiltersJson = JsonSerializer.Serialize(filters);
            ViewBag.CurrentFilter = currentFilter;
            return View();
        }

        private static List<object> BuildFilterConfig(Type entityType)
        {
            var filters = new List<object>();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (!IsFilterableType(type))
                    continue;

                var qbType = MapToQueryBuilderType(type);
                var filter = new Dictionary<string, object>
                {
                    { "id", prop.Name },
                    { "label", SplitCamelCase(prop.Name) },
                    { "type", qbType }
                };

                // Add specific operators based on type
                if (qbType == "string")
                {
                    filter["operators"] = new[] { "equal", "not_equal", "contains", "not_contains", "in", "not_in" };
                }
                else if (qbType == "integer" || qbType == "double")
                {
                    filter["operators"] = new[] { "equal", "not_equal", "greater", "greater_or_equal", "less", "less_or_equal", "in", "not_in" };
                }
                else if (qbType == "date" || qbType == "datetime")
                {
                    filter["operators"] = new[] { "equal", "not_equal", "greater", "greater_or_equal", "less", "less_or_equal" };
                }
                else if (qbType == "boolean")
                {
                    filter["operators"] = new[] { "equal" };
                    filter["input"] = "radio";
                    filter["values"] = new Dictionary<string, string> { { "true", "Yes" }, { "false", "No" } };
                }

                filters.Add(filter);
            }

            return filters;
        }

        private static bool IsFilterableType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }

        private static string MapToQueryBuilderType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte)) return "integer";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "double";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "datetime";
            if (type.IsEnum) return "string"; // Enums treated as string for UI
            return "string";
        }

        private static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "(\\B[A-Z])", " $1");
        }
    }
}
