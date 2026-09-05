using System.Text.Json;
using LifelogBb.Models.Entities;
using LifelogBb.Models.Filtering;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities
{
    public static class ControllerQueryExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Applies a FilterGroup expression tree to an IQueryable.
        /// The filterJson parameter is a JSON-serialized FilterGroup.
        /// Returns the original query unchanged if filterJson is null/empty.
        /// When throwOnInvalidFilter is true, invalid filter JSON/expression throws ArgumentException.
        /// </summary>
        public static IQueryable<T> FilterByGroup<T>(this IQueryable<T> query, string? filterJson, bool throwOnInvalidFilter = false)
        {
            if (string.IsNullOrWhiteSpace(filterJson))
                return query;

            FilterGroup? group;
            try
            {
                group = JsonSerializer.Deserialize<FilterGroup>(filterJson, JsonOptions);
            }
            catch (JsonException ex)
            {
                if (throwOnInvalidFilter)
                    throw new ArgumentException("Invalid filter JSON.", ex);
                return query;
            }

            if (group == null)
            {
                if (throwOnInvalidFilter)
                    throw new ArgumentException("Invalid filter JSON.");
                return query;
            }

            try
            {
                var predicate = DynamicFilterBuilder.BuildExpression<T>(group);
                if (predicate == null)
                    return query;

                return query.Where(predicate);
            }
            catch (ArgumentException)
            {
                if (throwOnInvalidFilter)
                    throw;
                return query;
            }
            catch (InvalidOperationException ex)
            {
                if (throwOnInvalidFilter)
                    throw new ArgumentException("Invalid filter expression.", ex);
                return query;
            }
        }

        /// <summary>
        /// The sort order SortByName actually applies, i.e. the requested one or the default it
        /// falls back to for a field the entity does not have. Kept separate because the tiebreakers
        /// below have to follow the direction of the order that is really in effect, not of the
        /// requested one.
        /// </summary>
        private static string ResolveSortOrder<T>(this IQueryable<T> query, string sortOrder, string defaultSort) where T : class
        {
            // Sorting name is not specified or on the entity => fallback to default to prevent errors
            if (string.IsNullOrEmpty(sortOrder) || query.ElementType.GetProperty(sortOrder.Replace("_desc", "")) == null)
            {
                return defaultSort;
            }

            return sortOrder;
        }

        // https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/advanced?view=aspnetcore-7.0#use-dynamic-linq-to-simplify-code
        public static IOrderedQueryable<T> SortByName<T>(this IQueryable<T> query, string sortOrder, string defaultSort = "CreatedAt_desc") where T : class
        {
            sortOrder = query.ResolveSortOrder(sortOrder, defaultSort);

            bool descending = false;
            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder[..^5];
                descending = true;
            }

            var ordered = descending
                ? query.OrderByDescending(e => EF.Property<object>(e, sortOrder))
                : query.OrderBy(e => EF.Property<object>(e, sortOrder));

            // Sorting by a field several rows share -- a day-granular Date above all, where one
            // workout is many sets -- leaves the tied rows in whatever order the database happens
            // to return them. A limited query then picks an arbitrary row out of that group and a
            // paged one is free to shuffle rows between pages, so "newest first" with limit 1 could
            // hand back the first set of the latest day instead of the last one logged. Logging
            // order breaks the tie, in the same direction as the chosen column, which keeps the
            // order stable and puts the newest entry actually first.
            foreach (var tiebreaker in new[] { nameof(BaseEntity.CreatedAt), nameof(BaseEntity.Id) })
            {
                if (string.Equals(sortOrder, tiebreaker, StringComparison.Ordinal)
                    || query.ElementType.GetProperty(tiebreaker) == null)
                    continue;

                var field = tiebreaker;
                ordered = descending
                    ? ordered.ThenByDescending(e => EF.Property<object>(e, field))
                    : ordered.ThenBy(e => EF.Property<object>(e, field));
            }

            return ordered;
        }

        public static IQueryable<T> FilterByStringProps<T>(this IQueryable<T> query, string field, string searchString) where T : class
        {
            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(searchString)) { return query; }

            var prop = query.ElementType.GetProperty(field);
            if (prop == null) { return query; }

            if (prop.PropertyType != typeof(string)) { return query; }

            return query.Where(e => EF.Property<string>(e, field).Contains(searchString));
        }

        public static IQueryable<T> FilterByDoubleProps<T>(this IQueryable<T> query, string field, string searchString, double range) where T : class
        {
            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(searchString)) { return query; }

            var prop = query.ElementType.GetProperty(field);
            if (prop == null) { return query; }

            // Accepts "80.5" and "80,5" alike, so the search box behaves the same on an en and a de machine.
            if (!NumberParsing.TryParseDouble(searchString, out var searchDouble))
            {
                return query;
            }

            return query.Where(e => EF.Property<double>(e, field) > searchDouble - range && EF.Property<double>(e, field) < searchDouble + range);
        }
    }
}
