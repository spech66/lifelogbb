using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Todos;
using LifelogBb.Utilities;

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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var defaultSortOrder = $"{nameof(Todo.CreatedAt)}_desc";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == nameof(Todo.CreatedAt) ? defaultSortOrder : nameof(Todo.CreatedAt);

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var todos = from s in _context.Todos select s;
            todos = todos.SortByName(sortOrder, defaultSortOrder);

            int pageSize = 20;
            return View(await PaginatedList<Todo>.CreateAsync(todos.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,IsCompleted,IsImportant,Category,Tags")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                todo.SetCreateFields();
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
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
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(todo);
        }

        // POST: Todos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Title,Description,DueDate,IsCompleted,IsImportant,Category,Tags,Id")] EditTodoViewModel todoViewModel)
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

            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
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
