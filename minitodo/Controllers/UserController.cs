using Microsoft.AspNetCore.Mvc;
using minitodo.Dtos;
using minitodo.Models;
using System.Security.Claims;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace minitodo.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Bad content";
                return View();//BadRequest("Bad content");
            }
            if (_context.Users.FirstOrDefault(a => a.Email == user.Email) != null)
            {
                ViewBag.Message = "Пользователь с таким email уже существует";
                return View();//BadRequest("User with the same email arleady exists");
            }
            User addUser = new User() { Name = user.Name, Email = user.Email, Surname = user.Surname, Password = GetHash(user.Password) };
            _context.Add(addUser);
            _context.SaveChanges();
            return RedirectToAction("Login");//Json(user);
        }
        //Get
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login( UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Неверный пароль или email!";
                return View();//BadRequest("Bad login and password!");
            }
            var user = _context.Users.FirstOrDefault(a => a.Email == userLogin.Email);
            if (user == null)
            {
                ViewBag.Message = "Нет пользователя с таким email";
                return View();//BadRequest("Bad email");
            }
            if (user.Password != GetHash(userLogin.Password))
            {
                ViewBag.Message = "Неверный пароль";
                return View();//BadRequest("Bad password");
            }
            UserModel userModel = new UserModel() { Name = user.Name, Id = user.Id, Password = "", Email = user.Email, Surname = user.Surname };
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync(claimsPrincipal);
            return RedirectToAction("Index", "Home");//Json(userModel);
        }
        [Authorize(Roles = "user")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");//Json("ok");
        }

        [Authorize(Roles = "user")]
        public IActionResult Delete([FromBody] UserLogin userLogin)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Неверный пароль или email!");
            }
            var user = _context.Users.FirstOrDefault(a => a.Email == userLogin.Email);
            if (user == null)
                return BadRequest("Нет пользователя с таким email");
            if (user.Password != GetHash(userLogin.Password))
            {
                return BadRequest("Неверный пароль");
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Json("ok");
        }

        [Authorize(Roles = "user")]
        public IActionResult GetAuthoriseInfo()
        {
            var email = HttpContext.User.Identity!.Name;
            var user = _context.Users.FirstOrDefault(a => a.Email == email);
            if (user == null)
                return BadRequest("user null");
            var userModel = new UserModel() { Name = user.Name, Email = user.Email, Surname = user.Surname, Id  = user.Id, Password = ""};
            return Json(userModel);
        }



        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }
}
