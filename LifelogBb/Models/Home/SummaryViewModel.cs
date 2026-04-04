using System;
using System.Collections.Generic;
using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Home
{
    public enum SummaryGranularity
    {
        Day,
        Week,
        Month,
        Year,
        All,
        Custom
    }

    public class SummaryViewModel
    {
        public SummaryGranularity Granularity { get; set; } = SummaryGranularity.Day;

        public DateTime Start { get; set; } = DateTime.Today;

        public DateTime End { get; set; } = DateTime.Today;

        public string RangeLabel { get; set; } = string.Empty;

        public int TotalEntries { get; set; }

        public int WorkoutsCount { get; set; }

        public int OpenTodosCount { get; set; }

        public int CompletedTodosCount { get; set; }

        public int ActiveGoalsCount { get; set; }

        public int CompletedGoalsCount { get; set; }

        public List<Weight> Weights { get; set; } = new();

        public List<Todo> Todos { get; set; } = new();

        public List<Goal> Goals { get; set; } = new();

        public List<Habit> Habits { get; set; } = new();

        public List<StrengthTraining> StrengthTrainings { get; set; } = new();

        public List<EnduranceTraining> EnduranceTrainings { get; set; } = new();

        public List<BucketList> BucketLists { get; set; } = new();

        public List<Quote> Quotes { get; set; } = new();

        public List<Journal> Journals { get; set; } = new();
    }
}
