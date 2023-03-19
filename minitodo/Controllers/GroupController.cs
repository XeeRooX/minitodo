using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minitodo.Models;
using System.Text.RegularExpressions;
using minitodo.Dtos;
namespace minitodo.Controllers
{
    public class GroupController : Controller
    {
        private readonly AppDbContext _context;
        public GroupController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult Create(string nameGroup)
        {
            var user = _context.Users.Find(1);
            if (nameGroup == null)
                return Json(false);

            var group = new Models.Group { Name = nameGroup, User = user };
            _context.Groups.Add(group);
            _context.SaveChanges();
            return Json(nameGroup);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (group == null || group.UserId != user.Id)
                return Json(false);
            _context.Groups.Remove(group);
            _context.SaveChanges();
            return Json(true);
        }
        [HttpPost]
        public JsonResult Edit(int id, string nameGroup)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (nameGroup == null || group == null || group.UserId != user.Id)
                return Json(false);

            group.Name = nameGroup;
            _context.SaveChanges();
            return Json(nameGroup);
        }
        [HttpGet]
        public JsonResult Read()
        {
            var user = _context.Users.Find(1);
            var groups = _context.Groups.Where(a => a.UserId == user.Id).ToList();
            var groupList = new List<GroupModel>();
            foreach (var group in groups)
            {
                groupList.Add(new GroupModel() { Id = group.Id, Name = group.Name });
            }
            return Json(groupList);
        }
    }
}
