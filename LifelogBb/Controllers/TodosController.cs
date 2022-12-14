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
using LifelogBb.Models.Quotes;
using LifelogBb.Models.Todos;

namespace LifelogBb.Controllers
{
    public class TodosController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public TodosController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Todos
        public async Task<IActionResult> Index()
        {
              return _context.Todos != null ? 
                          View(await _context.Todos.OrderByDescending(o => o.CreatedAt).ToListAsync()) :
                          Problem("Entity set 'LifelogBbContext.Todos'  is null.");
        }

        // GET: Todos/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Todos == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,IsCompleted,IsImportant")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                todo.SetCreateFields();
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todos/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Todos == null)
            {
                return NotFound();
            }

            var todoDb = await _context.Todos.FindAsync(id);
            if (todoDb == null)
            {
                return NotFound();
            }
            var todo = _mapper.Map<EditTodoViewModel>(todoDb);
            return View(todo);
        }

        // POST: Todos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Title,Description,DueDate,IsCompleted,IsImportant,Id")] EditTodoViewModel todoViewModel)
        {
            if (id != todoViewModel.Id)
            {
                return NotFound();
            }

            var todoDb = await _context.Todos.FindAsync(id);
            if (ModelState.IsValid && todoDb != null)
            {
                try
                {
                    todoDb = _mapper.Map(todoViewModel, todoDb);
                    todoDb.SetUpdateFields();
                    _context.Update(todoDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todoViewModel.Id))
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
            return View(todoViewModel);
        }

        // GET: Todos/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Todos == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Todos == null)
            {
                return Problem("Entity set 'LifelogBbContext.Todos'  is null.");
            }
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(long id)
        {
          return (_context.Todos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
