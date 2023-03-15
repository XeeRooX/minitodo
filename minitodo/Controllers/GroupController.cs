using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minitodo.Models;

namespace minitodo.Controllers
{
    public class GroupController : Controller
    {
        private readonly AppDbContext _context;
        public GroupController(AppDbContext context)
        {
            _context = context;
        }

        public string Putin()
        {
            return "Vor";
        }
        [HttpPost]
        public JsonResult Create(string nameGroup)
        {
            return Json(new{name = nameGroup });

            var user = _context.Users.Find(1);
            if (nameGroup == null)
                return null;

            var group = new Group { Name = nameGroup, User = user };
            _context.Groups.Add(group);
            _context.SaveChanges();
            return Json(nameGroup);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (group == null || group.UserId != user.Id)
                return null;
            _context.Groups.Remove(group);
            _context.SaveChanges();
            return Json(true);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, string nameGroup)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (nameGroup == null || group==null||group.UserId!=user.Id)
                return null;

            group.Name = nameGroup;
            _context.SaveChanges();
            return Json(nameGroup);
        }
        [HttpGet]
        public JsonResult Read()
        {
            var user = _context.Users.Find(1);
            var groupList = _context.Groups.Include(a => a.Tasks).Where(a => a.UserId == user.Id);

            return Json(groupList);
        }
    }
}
