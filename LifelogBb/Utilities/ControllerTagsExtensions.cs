using LifelogBb.Models;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Utilities
{
    public static class ControllerTagsExtensions
    {
        public static void AddTagsToViewData(this Controller controller, LifelogBbContext _context)
        {
            var tagGoals = _context.Goals.Select(s => s.Tags).Distinct();
            var tagsHabits = _context.Habits.Select(s => s.Tags).Distinct();
            var tagsJournal = _context.Journals.Select(s => s.Tags).Distinct();
            var tagsQuotes = _context.Quotes.Select(s => s.Tags).Distinct();
            var tagsTodos = _context.Todos.Select(s => s.Tags).Distinct();
            var tagsBucketLists = _context.BucketLists.Select(s => s.Tags).Distinct();

            var tags = tagGoals.Union(tagsHabits).Union(tagsJournal).Union(tagsQuotes).Union(tagsTodos).Union(tagsBucketLists).Distinct();            
            var tagsList = tags.ToList().Where(s => s != null).SelectMany(s => s.Split(',')).Distinct();
            var tagsText = string.Join(",", tagsList);
            
            controller.ViewData["TagsList"] = tagsText;
        }

        public static void AddCategoriesToViewData(this Controller controller, LifelogBbContext _context)
        {
            var tagGoals = _context.Goals.Select(s => s.Category).Distinct();
            var tagsHabits = _context.Habits.Select(s => s.Category).Distinct();
            var tagsJournal = _context.Journals.Select(s => s.Category).Distinct();
            var tagsQuotes = _context.Quotes.Select(s => s.Category).Distinct();
            var tagsTodos = _context.Todos.Select(s => s.Category).Distinct();
            var tagsBucketLists = _context.BucketLists.Select(s => s.Category).Distinct();

            var categories = tagGoals.Union(tagsHabits).Union(tagsJournal).Union(tagsQuotes).Union(tagsTodos).Union(tagsBucketLists).Distinct();
            var categoriesList = categories.ToList().Where(s => s != null).Distinct();
            var categoriessText = string.Join(",", categoriesList);

            controller.ViewData["CategoryList"] = categoriessText;
        }
    }
}
