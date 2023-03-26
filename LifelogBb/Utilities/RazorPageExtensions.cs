using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LifelogBb.Utilities
{
    public static class RazorPageExtensions
    {
        public static string GetSortOrder<T>(ViewDataDictionary<T> viewData, string fieldName)
        {
            if (viewData["CurrentSort"] == null)
            {
                return "";
            }

            if ((string)viewData["CurrentSort"] == $"{fieldName}_desc")
            {
                return "desc";
            }

            else if ((string)viewData["CurrentSort"] == fieldName)
            {
                return "asc";
            }

            return "";
        }
    }
}
