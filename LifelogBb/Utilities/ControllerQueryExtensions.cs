using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities
{
    public static class ControllerQueryExtensions
    {
        // https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/advanced?view=aspnetcore-7.0#use-dynamic-linq-to-simplify-code
        public static IOrderedQueryable<T> SortByName<T>(this IQueryable<T> query, string sortOrder, string defaultSort = "CreatedAt_desc")
        {
            // Sorting name is not specified or on the entity => fallback to default to prevent errors
            if (string.IsNullOrEmpty(sortOrder) || query.ElementType.GetProperty(sortOrder.Replace("_desc", "")) == null)
            {
                sortOrder = defaultSort;
            }

            bool descending = false;
            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder[..^5];
                descending = true;
            }

            if (descending)
            {
                return query.OrderByDescending(e => EF.Property<object>(e, sortOrder));
            }
            else
            {
                return query.OrderBy(e => EF.Property<object>(e, sortOrder));
            }
        }

        public static IQueryable<T> FilterByStringProps<T>(this IQueryable<T> query, string field, string searchString)
        {
            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(searchString)) { return query; }

            var prop = query.ElementType.GetProperty(field);
            if (prop == null) { return query; }

            if (prop.PropertyType != typeof(string)) { return query; }

            return query.Where(e => EF.Property<string>(e, field).Contains(searchString));
        }

        public static IQueryable<T> FilterByDoubleProps<T>(this IQueryable<T> query, string field, string searchString, double range)
        {
            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(searchString)) { return query; }

            var prop = query.ElementType.GetProperty(field);
            if (prop == null) { return query; }

            if (!double.TryParse(searchString, out var searchDouble))
            {
                return query;
            }

            return query.Where(e => EF.Property<double>(e, field) > searchDouble - range && EF.Property<double>(e, field) < searchDouble + range);
        }
    }
}
