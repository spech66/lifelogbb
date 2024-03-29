﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LifelogBb.Utilities
{
    public static class RazorPageExtensions
    {
        /// <summary>
        /// Get desc or asc depending of the current sort order of the field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewData"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetSortOrder<T>(ViewDataDictionary<T> viewData, string fieldName)
        {
            if (viewData["CurrentSort"] == null)
            {
                return "";
            }

            if (viewData["CurrentSort"] as string == $"{fieldName}_desc")
            {
                return "desc";
            } else if (viewData["CurrentSort"] as string == fieldName)
            {
                return "asc";
            }

            return "";
        }

        /// <summary>
        /// Get the inverted route for the field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewData"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetSortRoute<T>(ViewDataDictionary<T> viewData, string fieldName)
        {
            if (viewData.ContainsKey("CurrentSort") && viewData["CurrentSort"] != null && viewData["CurrentSort"] as string == $"{fieldName}_desc")
            {
                return fieldName;
            }

            return $"{fieldName}_desc";
        }
    }
}
