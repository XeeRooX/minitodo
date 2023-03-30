﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create(string nameGroup)
        {
            var user = _context.Users.Find(1);
            if (nameGroup == null)
                return BadRequest("no name group");
            if(_context.Groups.FirstOrDefault(a=>a.Name == nameGroup) != null)
                return BadRequest("group already exists");
            var group = new Models.Group { Name = nameGroup, User = user };
            _context.Groups.Add(group);
            _context.SaveChanges();
            return Json(new {id = group.Id, name = nameGroup });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (group == null || group.UserId != user.Id)
                return BadRequest("no group");
            _context.Groups.Remove(group);
            _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public IActionResult Edit(int id, string nameGroup)
        {
            var user = _context.Users.Find(1);
            var group = _context.Groups.Find(id);
            if (nameGroup == null || group == null || group.UserId != user.Id)
                return BadRequest("no group");

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
