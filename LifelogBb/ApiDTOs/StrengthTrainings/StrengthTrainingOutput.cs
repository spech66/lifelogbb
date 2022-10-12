﻿using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.StrengthTrainings
{
    public class StrengthTrainingOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string? Exercise { get; set; }

        public int Reps { get; set; }

        public decimal Weight { get; set; }

        public string? Notes { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
