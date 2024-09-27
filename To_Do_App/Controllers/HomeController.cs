using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using To_Do_App.Data;
using To_Do_App.Models;

namespace To_Do_App.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbcontext _context;

        public HomeController(AppDbcontext context)
        {
            _context = context;
        }

        // عرض قائمة المهام مع خيارات التصفية
        public async Task<IActionResult> Index(Filter filter)
        {
            var todosQuery = _context.todos.Include(t => t.Category).Include(t => t.GetStatus).AsQueryable();

            // تصفية المهام حسب الفئة إذا تم تحديد فئة
            if (!string.IsNullOrEmpty(filter.CategoryId))
            {
                todosQuery = todosQuery.Where(t => t.CategoryId == filter.CategoryId);
            }

            // تصفية المهام حسب الحالة إذا تم تحديد حالة
            if (!string.IsNullOrEmpty(filter.StatusId))
            {
                todosQuery = todosQuery.Where(t => t.StatusId == filter.StatusId);
            }

            var todos = await todosQuery.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Statuses = await _context.statuses.ToListAsync();
            ViewBag.SelectedFilter = filter; // تمرير الفلتر إلى العرض
            return View(todos);
        }

        // GET: Create Task
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.statuses.ToList();
            return View();
        }

        // POST: Create Task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {

            todo.Id = Guid.NewGuid().ToString(); // Generate a new unique ID
            await _context.todos.AddAsync(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect to the task list after creation

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.statuses.ToList();
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(string id, Todo todo)
        {
            // Find the task by its ID
            var taskToUpdate = await _context.todos.FindAsync(id);
            if (taskToUpdate == null)
            {
                return NotFound();
            }

            // Toggle the IsOpened property
            taskToUpdate.IsOpened = todo.IsOpened;

            // Change the status based on whether the task is opened or completed
            taskToUpdate.StatusId = todo.IsOpened ? "opend" : "close";

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.statuses.ToList();
          

            return View(todo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // To prevent CSRF attacks
        public async Task<IActionResult> Edit(Todo todo)
        {
                // Check if the todo item exists
                var existingTodo = await _context.todos.FindAsync(todo.Id);
                if (existingTodo == null)
                {
                    return NotFound(); // If it doesn't exist, return a 404 Not Found
                }
            else
            {
                // Update the properties of the existing todo with the new values
                existingTodo.Description = todo.Description;
                existingTodo.Date = todo.Date;
                existingTodo.CategoryId = todo.CategoryId;
                existingTodo.StatusId = todo.StatusId;
                existingTodo.IsOpened = todo.IsOpened; // Update the IsOpened property if applicable

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the index action after successful edit
                return RedirectToAction("Index");


                // If model state is not valid, repopulate the ViewBag
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Statuses = await _context.statuses.ToListAsync();
            }
           

            // Return the view with the model if validation fails
            return View(todo);
        }

        // GET: Todos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.todos
                .Include(t => t.Category)
                .Include(t => t.GetStatus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Home/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var todo = await _context.todos.FindAsync(id);
            if (todo != null)
            {
                _context.todos.Remove(todo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearCompleted()
        {
            var completedTodos = await _context.todos
                .Where(t => t.StatusId == "close") // Adjust based on your completed status ID
                .ToListAsync();

            if (completedTodos.Any())
            {
                _context.todos.RemoveRange(completedTodos);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index");
        }




    }
}

