using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly IMongoCollection<TaskItem> _taskCollection;

        public TaskController(IMongoClient client)
        {
            var database = client.GetDatabase("TaskManagment");
            _taskCollection = database.GetCollection<TaskItem>("Tasks");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.Name;
            var tasks = await _taskCollection.Find(t => t.UserId == userId).SortBy(t => t.StartDate).ToListAsync();
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string title, DateTime startDate)
        {
            var userId = User.Identity?.Name;
            await _taskCollection.InsertOneAsync(new TaskItem
            {
                UserId = userId,
                Title = title,
                StartDate = startDate
            });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _taskCollection.DeleteOneAsync(t => t.Id == id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            await _taskCollection.UpdateOneAsync(
                t => t.Id == id,
                Builders<TaskItem>.Update.Set(t => t.Status, status)
            );
            return RedirectToAction("Index");
        }
    }
}
