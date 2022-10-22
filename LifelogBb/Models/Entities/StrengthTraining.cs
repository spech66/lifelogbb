﻿using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class StrengthTraining : BaseEntity
    {
        [Required]
        [MinLength(1)]
        public string? Exercise { get; set; }

        public int Reps { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Weight { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
