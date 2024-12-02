using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserController(IMongoClient client)
        {
            var database = client.GetDatabase("TaskManagment");
            _userCollection = database.GetCollection<User>("Users");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userCollection.Find(u => u.Username == username && u.Password == password).FirstOrDefaultAsync();
            if (user != null)
            {
                HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id) }, CookieAuthenticationDefaults.AuthenticationScheme)
                ));
                return RedirectToAction("Index", "Task");
            }
            ViewBag.Message = "Invalid username or password.";
            return View();
        }

        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(string username, string password)
        {
            var userExists = await _userCollection.Find(u => u.Username == username).AnyAsync();
            if (!userExists)
            {
                await _userCollection.InsertOneAsync(new User { Username = username, Password = password });
                return RedirectToAction("Login");
            }
            ViewBag.Message = "User already exists.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
