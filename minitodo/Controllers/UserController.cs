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

        public IActionResult Create([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad content");
            }
            if (_context.Users.FirstOrDefault(a => a.Email == user.Email) != null)
            {
                return BadRequest("User with the same email arleady exists");
            }
            User addUser = new User() { Name = user.Name, Email = user.Email, Surname = user.Surname, Password = GetHash(user.Password) };
            _context.Add(addUser);
            _context.SaveChanges();
            return Json(user);
        }
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad login and password!");
            }
            var user = _context.Users.FirstOrDefault(a => a.Email == userLogin.Email);
            if (user == null)
                return BadRequest("Bad email");
            if (user.Password != GetHash(userLogin.Password))
            {
                return BadRequest("Bad password");
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
            return Json(userModel);
        }
        [Authorize(Roles = "user")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json("ok");
        }

        [Authorize(Roles = "user")]
        public IActionResult Delete([FromBody] UserLogin userLogin)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Bad login and password!");
            }
            var user = _context.Users.FirstOrDefault(a => a.Email == userLogin.Email);
            if (user == null)
                return BadRequest("Bad email");
            if (user.Password != GetHash(userLogin.Password))
            {
                return BadRequest("Bad password");
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Json("ok");
        }

        [Authorize(Roles = "user")]
        public IActionResult GetAuthoriseInfo()
        {
            var email = HttpContext.User.Identity.Name;
            var user = _context.Users.FirstOrDefault(a => a.Email == email);
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
