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
            if (string.IsNullOrEmpty(sortOrder) || query.ElementType.GetProperty(sortOrder.Replace("_dec", "")) == null)
            {
                sortOrder = defaultSort;
            }

            bool descending = false;
            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder.Substring(0, sortOrder.Length - 5);
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
    }
}
