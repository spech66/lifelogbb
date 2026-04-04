using LifelogBb.Utilities;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;

namespace LifelogBb.Views.Shared.Components.Navigation
{
    public class Navigation : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var list = new List<NavItem>
            {
                new NavItem("Home", "Index", "Home", "fas fa-home"),
                new NavItem("Home", "Dashboard", "Dashboard", "fa-solid fa-chalkboard"),
                new NavItem("Home", "Calendar", "Calendar", "fas fa-calendar-day"),
                new NavItem("Home", "Summary", "Summary", "fa-solid fa-table-cells-large"),
                new NavItem("Weight", "fas fa-weight-scale", new List<NavItem>
                {
                    new NavItem("Weights", "Index", "Weight", "fas fa-weight-scale"),
                    new NavItem("Weights", "Graph", "Graph", "fas fa-chart-line"),
                    new NavItem("Weights", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Journals", "fas fa-pencil", new List<NavItem>
                {
                    new NavItem("Journals", "Index", "Journal", "fas fa-pencil"),
                    new NavItem("Journals", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Strength training", "fas fa-dumbbell", new List<NavItem>
                {
                    new NavItem("StrengthTrainings", "Index", "Strength training", "fas fa-dumbbell"),
                    new NavItem("StrengthTrainings", "Graph", "Graph", "fas fa-chart-line"),
                    new NavItem("StrengthTrainings", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Endurance training", "fas fa-running", new List <NavItem>
                {
                    new NavItem("EnduranceTrainings", "Index", "Endurance training", "fas fa-running"),
                    new NavItem("EnduranceTrainings", "Graph", "Graph", "fas fa-chart-line"),
                    new NavItem("EnduranceTrainings", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Todos", "fas fa-check", new List<NavItem>
                {
                    new NavItem("Todos", "Index", "Todos", "fas fa-check"),
                    new NavItem("Todos", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Goals", "fas fa-bullseye", new List<NavItem>
                {
                    new NavItem("Goals", "Index", "Goals", "fas fa-bullseye"),
                    new NavItem("Goals", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Habits", "fas fa-arrows-spin", new List<NavItem>
                {
                    new NavItem("Habits", "Index", "Habits", "fas fa-arrows-spin"),
                    new NavItem("Habits", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Bucket list", "fas fa-tent", new List <NavItem>
                {
                    new NavItem("BucketLists", "Index", "Bucket list", "fas fa-tent"),
                    new NavItem("BucketLists", "VisionBoard", "Vision board", "fas fa-chart-bar icon"),
                    new NavItem("BucketLists", "Table", "Table", "fas fa-table"),
                }),
                new NavItem("Quotes", "fas fa-quote-right", new List<NavItem>
                {
                    new NavItem("Quotes", "Index", "Quotes", "fas fa-quote-right"),
                    new NavItem("Quotes", "Random", "Random", "fas fa-shuffle"),
                    new NavItem("Quotes", "Table", "Table", "fas fa-table"),
                }),
            };

            return View(list);
        }
    }
}
