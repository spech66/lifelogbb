﻿using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class Weight : BaseEntity
    {
        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0}")]
        [DisplayName("Weight")]
        [Range(40, 440)] // 200 Kg => 440 lbs...
        public double BodyWeight { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DisplayName("BMI")]
        public double Bmi { get; set; }

        [DisplayName("BMI Overweight")]
        public double BmiOverweight => 25.0;

        [DisplayName("BMI Underweight")]
        public double BmiUnderweight => 18.5;

        public Weight()
        {
            // Default constructor
        }

        public Weight(int height, double bodyWeight)
        {
            Height = height;
            BodyWeight = bodyWeight;
        }
    }
}
