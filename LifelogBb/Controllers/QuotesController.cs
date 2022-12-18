using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Journals;
using LifelogBb.Models.Quotes;

namespace LifelogBb.Controllers
{
    public class QuotesController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public QuotesController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Quotes
        public async Task<IActionResult> Index()
        {
              return _context.Quotes != null ? 
                          View(await _context.Quotes.OrderByDescending(o => o.CreatedAt).ToListAsync()) :
                          Problem("Entity set 'LifelogBbContext.Quotes'  is null.");
        }

        // GET: Quotes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Quotes == null)
            {
                return NotFound();
            }

            var quote = await _context.Quotes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        // GET: Quotes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,Author")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                quote.SetCreateFields();
                _context.Add(quote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quote);
        }

        // GET: Quotes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Quotes == null)
            {
                return NotFound();
            }

            var quoteDb = await _context.Quotes.FindAsync(id);
            if (quoteDb == null)
            {
                return NotFound();
            }
            var quote = _mapper.Map<EditQuoteViewModel>(quoteDb);
            return View(quote);
        }

        // POST: Quotes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Text,Author,Id")] EditQuoteViewModel quoteViewModel)
        {
            if (id != quoteViewModel.Id)
            {
                return NotFound();
            }

            var quoteDb = await _context.Quotes.FindAsync(id);
            if (ModelState.IsValid && quoteDb != null)
            {
                try
                {
                    quoteDb = _mapper.Map(quoteViewModel, quoteDb);
                    quoteDb.SetUpdateFields();
                    _context.Update(quoteDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuoteExists(quoteViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(quoteViewModel);
        }

        // GET: Quotes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Quotes == null)
            {
                return NotFound();
            }

            var quote = await _context.Quotes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Quotes == null)
            {
                return Problem("Entity set 'LifelogBbContext.Quotes'  is null.");
            }
            var quote = await _context.Quotes.FindAsync(id);
            if (quote != null)
            {
                _context.Quotes.Remove(quote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuoteExists(long id)
        {
          return (_context.Quotes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
