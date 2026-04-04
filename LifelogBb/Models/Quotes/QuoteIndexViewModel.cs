using LifelogBb.Models.Entities;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Quotes
{
    public class QuoteIndexViewModel
    {
        public int TotalCount { get; set; }

        public int WithAuthorCount { get; set; }

        public int UniqueAuthorsCount { get; set; }

        public int TaggedCount { get; set; }

        public required PaginatedList<Quote> List { get; set; }
    }
}
