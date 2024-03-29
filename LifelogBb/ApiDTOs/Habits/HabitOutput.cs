﻿using LifelogBb.Interfaces.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Habits
{
    public class HabitOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? RecurrenceRules { get; set; }

        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
