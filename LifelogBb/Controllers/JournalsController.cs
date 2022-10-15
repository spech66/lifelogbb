﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.EnduranceTrainings;
using LifelogBb.Models.Journals;

namespace LifelogBb.Controllers
{
    public class JournalsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public JournalsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Journals
        public async Task<IActionResult> Index()
        {
              return View(await _context.Journals.OrderByDescending(o => o.CreatedAt).ToListAsync());
        }

        // GET: Journals/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // GET: Journals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Journals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text")] Journal journal)
        {
            if (ModelState.IsValid)
            {
                journal.CreatedAt = journal.UpdatedAt = DateTime.Now;
                _context.Add(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(journal);
        }

        // GET: Journals/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journalDb = await _context.Journals.FindAsync(id);
            if (journalDb == null)
            {
                return NotFound();
            }
            var journal = _mapper.Map<EditJournalViewModel>(journalDb);
            return View(journal);
        }

        // POST: Journals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Text,Id")] EditJournalViewModel journalViewModel)
        {
            if (id != journalViewModel.Id)
            {
                return NotFound();
            }

            var journalDb = await _context.Journals.FindAsync(id);
            if (ModelState.IsValid && journalDb != null)
            {
                try
                {
                    journalDb = _mapper.Map(journalViewModel, journalDb);
                    journalDb.UpdatedAt = DateTime.Now;
                    _context.Update(journalDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalExists(journalViewModel.Id))
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
            return View(journalViewModel);
        }

        // GET: Journals/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // POST: Journals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Journals == null)
            {
                return Problem("Entity set 'LifelogBbContext.Journals'  is null.");
            }
            var journal = await _context.Journals.FindAsync(id);
            if (journal != null)
            {
                _context.Journals.Remove(journal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalExists(long id)
        {
          return _context.Journals.Any(e => e.Id == id);
        }
    }
}