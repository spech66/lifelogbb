using AutoMapper;
using LifelogBb.Models;
using LifelogBb.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LifelogBb.Controllers
{
    public class HomeController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;

        public HomeController(LifelogBbContext context, IMapper mapper, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new IndexDashboardViewModel();

            var enduranceTraining = await _context.EnduranceTrainings.OrderByDescending(o => o.CreatedAt).Take(1).FirstOrDefaultAsync();
            if (enduranceTraining != null)
            {
                model.LastEnduranceTraining = enduranceTraining.CreatedAt;
            }

            var strengthTraining = await _context.StrengthTrainings.OrderByDescending(o => o.CreatedAt).Take(1).FirstOrDefaultAsync();
            if (strengthTraining != null)
            {
                model.LastStrengthTraining = strengthTraining.CreatedAt;
            }

            var randomBucketList = await _context.BucketLists.OrderBy(r => EF.Functions.Random()).Take(1).FirstOrDefaultAsync();
            if (randomBucketList != null)
            {
                model.RandomBucketList = randomBucketList;
            }

            var randomQuote = await _context.Quotes.OrderBy(r => EF.Functions.Random()).Take(1).FirstOrDefaultAsync();
            if (randomQuote != null)
            {
                model.RandomQuote = randomQuote;
            }

            return View(model);
        }

        public IActionResult Config()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*private Quote RandomQuote()
        {
            Quote[] quotes =
            {
                new Quote { Text = "Success is the product of daily habits - not once-in-a-lifetime transformations.", Cite = "James Clear" },
                new Quote { Text = "Whoever is happy will make others happy too.", Cite = "Anne Frank" },
                new Quote { Text = "You only live once, but if you do it right, once is enough.", Cite = "Mae West" },
                new Quote { Text = "Twenty years from now you will be more disappointed by the things that you didn't do than by the ones you did do.", Cite = "Mark Twain" },
                new Quote { Text = "The greatest glory in living lies not in never falling, but in rising every time we fall.", Cite = "Nelson Mandela" },
                new Quote { Text = "A Year From Now You Will Wish You Had Started Today.", Cite = "Karen Lamb" },
                new Quote { Text = "Are you living your life or just waiting to die?", Cite = "Steve Aoki" },
                new Quote { Text = "There's a sunrise and a sunset every single day, and they're absolutely free. Don't miss so many of them.", Cite = "Jo Walton" },
                new Quote { Text = "You don't always need a plan. Sometimes you just need to breathe, trust, let go, and see what happens.", Cite = "Mandy Hale" },
                new Quote { Text = "Do not dwell in the past, do not dream of the future, concentrate the mind on the present moment.", Cite = "Buddha" },
            };
            var random = new Random();
            int index = random.Next(quotes.Count());
            return quotes[index];
        }*/
    }
}
