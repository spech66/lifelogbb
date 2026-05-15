using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using LifelogBb.Models.Filtering;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Views.Shared.Components.FilterBuilder
{
    public class FilterBuilder : ViewComponent
    {
        private static readonly JsonSerializerOptions CurrentFilterJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly ConcurrentDictionary<Type, string> FilterConfigJsonCache = new();

        public IViewComponentResult Invoke(Type entityType, string? currentFilter = null)
        {
            ViewBag.FiltersJson = FilterConfigJsonCache.GetOrAdd(entityType,
                static type => JsonSerializer.Serialize(BuildFilterConfig(type)));
            ViewBag.CurrentFilterJson = NormalizeCurrentFilterJson(currentFilter);
            return View();
        }

        private static string? NormalizeCurrentFilterJson(string? currentFilter)
        {
            if (string.IsNullOrWhiteSpace(currentFilter))
                return null;

            try
            {
                var parsed = JsonSerializer.Deserialize<FilterGroup>(currentFilter, CurrentFilterJsonOptions);
                if (parsed == null || !IsValidFilterGroup(parsed))
                    return null;

                return JsonSerializer.Serialize(parsed, CurrentFilterJsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static bool IsValidFilterGroup(FilterGroup group)
        {
            if (group.Conditions == null || group.Groups == null)
                return false;

            foreach (var condition in group.Conditions)
            {
                if (condition == null || condition.Field == null || condition.Value == null)
                    return false;
            }

            foreach (var subGroup in group.Groups)
            {
                if (subGroup == null || !IsValidFilterGroup(subGroup))
                    return false;
            }

            return true;
        }

        private static List<object> BuildFilterConfig(Type entityType)
        {
            var filters = new List<object>();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.SetMethod == null || prop.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

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

                // Add specific operators based on actual CLR type
                if (type == typeof(string))
                {
                    filter["operators"] = new[] { "equal", "not_equal", "contains", "not_contains", "in", "not_in" };
                }
                else if (type.IsEnum || type == typeof(Guid) || type == typeof(TimeSpan))
                {
                    filter["operators"] = new[] { "equal", "not_equal", "in", "not_in" };

                    if (type.IsEnum)
                    {
                        filter["input"] = "select";
                        filter["values"] = Enum.GetNames(type).ToDictionary(n => n, n => SplitCamelCase(n));
                    }
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
