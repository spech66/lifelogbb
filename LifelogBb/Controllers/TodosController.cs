using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Todos;
using LifelogBb.Utilities;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Ical.Net;
using CalendarComponents = Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

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
            ViewData["CurrentSort"] = sortOrder;

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
            todos = todos.SortByName(sortOrder, $"{nameof(Todo.CreatedAt)}_desc");

            var config = Config.GetConfig(_context);
            ViewData["FeedToken"] = config.FeedToken;
            var list = await PaginatedList<Todo>.CreateAsync(todos.AsNoTracking(), pageNumber ?? 1, config.TodoPageSize);
            return View(new PaginatedListViewModel<Todo>(list, config));
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
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,IsCompleted,IsImportant,Category,Tags,StartDate,Progress,Progress")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                todo.SetCreateFields();
                if (todo.Completed == null && todo.IsCompleted)
                {
                    todo.Completed = DateTime.Now;
                }
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
        public async Task<IActionResult> Edit(long id, [Bind("Title,Description,DueDate,IsCompleted,IsImportant,Category,Tags,StartDate,Progress,Progress,Id")] EditTodoViewModel todoViewModel)
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
                    if(todoViewModel.Completed == null && todoViewModel.IsCompleted)
                    {
                        todoDb.Completed = DateTime.Now;
                    }
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
                return Problem("Entity set 'LifelogBbContext.Todos' is null.");
            }
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IResult> Feed(string token)
        {
            var config = Config.GetConfig(_context);
            if (config == null || config.FeedToken == null || config.FeedToken != token)
            {
                return Results.Content("Unauthorized", contentType: "text/plain");
            }

            var calendar = new Calendar();
            calendar.AddTimeZone(new CalendarComponents.VTimeZone(config.FeedTimeZone));

            var todosQuery = from s in _context.Todos select s;
            var todos = await todosQuery.ToListAsync();
            todos.ToList().ForEach(todo =>
            {
                calendar.Todos.Add(new CalendarComponents.Todo()
                {
                    Uid = todo.Id.ToString(),
                    Url = new Uri(Url.Action(nameof(Details), nameof(TodosController).Replace("Controller", ""), new { id = todo.Id }, "https", Request.Host.Value)),
                    Summary = todo.Title,
                    Description = todo.Description,
                    Completed = todo.Completed.HasValue ? new CalDateTime(todo.Completed.Value) : null,
                    Start = todo.StartDate.HasValue ? new CalDateTime(todo.StartDate.Value) : null,
                    Priority = todo.IsImportant ? 1 : 5, // 0-9, 0=undefined, 1=highest, 9=lowest
                    Status = todo.IsCompleted || todo.Completed.HasValue ? "COMPLETED" : (todo.Progress > 0 ? "IN-PROCESS" : ""),
                    Categories = new List<string>() { todo.Category ?? "" },
                    PercentComplete = todo.Progress, // 0 = not started, 1=100
                });
            });

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            return Results.Content(serializedCalendar, contentType: "text/calendar");
        }

        private bool TodoExists(long id)
        {
          return (_context.Todos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
