﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.EnduranceTrainings
{
    public class EditEnduranceTrainingViewModel
    {
        public long Id { get; set; }

        public string? Exercise { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Distance { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? Duration { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}