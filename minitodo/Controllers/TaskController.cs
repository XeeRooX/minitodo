using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minitodo.Dtos;
using minitodo.Models;

namespace minitodo.Controllers
{
    public class TaskController : Controller
    {
        private AppDbContext db;
        public TaskController(AppDbContext db)
        {
            this.db = db;
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] TaskCreate task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad content");
            }
            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");
            int userId = user.Id;
            var group = db.Groups.Include(g => g.Tasks).FirstOrDefault(g => g.UserId == userId && g.Id == task.GroupId);
            if( group == null)
            {
                return BadRequest("Group not exist");
            }

            var task_model = new Models.Task()
            {
                Description = task.TaskTitle
            };
            group.Tasks.Add(task_model);
            db.SaveChanges();

            return Json(new
            {
                taskId = task_model.Id,
                groupId = task_model.GroupId,
                description = task_model.Description
            });
        }
        [Authorize]
        [HttpDelete]
        public IActionResult Delete([FromBody] TaskDelete taskInfo)
        {
            if (!ModelState.IsValid || taskInfo.TaskId == 0)
            {
                return BadRequest("Bad content");
            }
            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");
            int userId = user.Id;
            //var task = db.Tasks.Find(taskInfo.TaskId);
            var task = db.Tasks.Include(t => t.Group).FirstOrDefault(t => t.Group.UserId == userId && t.Id == taskInfo.TaskId);
            
            if (task == null)
                return BadRequest("Not exist");

            db.Tasks.Remove(task);
            db.SaveChanges();

            return Ok();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit([FromBody] TaskEdit taskInfo)
        {
            if (!ModelState.IsValid || taskInfo.TaskId == 0)
            {
                return BadRequest("Bad content");
            }

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");
            
            int userId = user.Id;
            //var task = db.Tasks.Find(taskInfo.TaskId);
            var task = db.Tasks.Include(t => t.Group).FirstOrDefault(t=> t.Group.UserId == userId && t.Id == taskInfo.TaskId);
            if (task == null)
            {
                return BadRequest("Not exist");
            }

            task.Description = taskInfo.TaskTitle;
            db.SaveChanges();

            return Json(taskInfo);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll([FromBody] GetTask groupInfo)
        {
            if (groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;
            var group = db.Groups.Include(g => g.Tasks).FirstOrDefault(g => g.Id == groupInfo.GroupId && g.UserId == userId);
            
            if (group == null)
            {
                return BadRequest("Group not exist");
            }

            var tasks = group.Tasks;
            List<GetTaskResult> allTasks = new();
            tasks.ForEach(t => allTasks.Add(new GetTaskResult()
            {
                Id = t.Id,
                Description = t.Description,
                IsDone = t.IsDone,
                IsFavorite = t.IsFavorite
            }));

            return Json(allTasks);
        }
        [Authorize]
        [HttpPost]
        public IActionResult GetConfirmed([FromBody] GetTask groupInfo)
        {
            if (groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;
            var group = db.Groups.Include(g => g.Tasks).FirstOrDefault(g=> g.UserId == userId && g.Id == groupInfo.GroupId);
            
            if (group == null)
            {
                return BadRequest("Group not exist");
            }

            var tasks = group.Tasks.Where(t => t.IsDone == true).ToList();
            List<GetTaskResult> confTasks = new();

            tasks.ForEach(t => confTasks.Add(new GetTaskResult()
            {
                Id = t.Id,
                Description = t.Description,
                IsDone = t.IsDone,
                IsFavorite = t.IsFavorite
            }));

            return Json(confTasks);
        }

        [Authorize]
        [HttpPost]
        public IActionResult GetNotConfirmed([FromBody] GetTask groupInfo)
        {
            if (groupInfo == null || groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;

            var group = db.Groups.Include(g=>g.Tasks).FirstOrDefault(g => g.UserId == userId && g.Id == groupInfo.GroupId);
            if (group == null)
            {
                return BadRequest("Group not exist");
            }

            var tasks = group.Tasks.Where(t => t.IsDone == false).ToList();
            List<GetTaskResult> confTasks = new();

            tasks.ForEach(t => confTasks.Add(new GetTaskResult()
            {
                Id = t.Id,
                Description = t.Description,
                IsDone = t.IsDone,
                IsFavorite = t.IsFavorite
            }));

            return Json(confTasks);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetFavorited()
        {

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;

            var tasks = db.Tasks.Include(t => t.Group.User).Where(t=> t.Group.UserId == userId && t.IsFavorite == true).ToList();
            List<GetTaskResult> favTasks = new();

            tasks.ForEach(t => favTasks.Add(new GetTaskResult()
            {
                Id = t.Id,
                Description = t.Description,
                IsDone = t.IsDone,
                IsFavorite = t.IsFavorite
            }));

            return Json(favTasks);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetFavorited([FromBody]SetTask taskInfo)
        {
            if (taskInfo.TaskId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;

            var task = db.Tasks.Include(t => t.Group.User).FirstOrDefault(t => t.Group.UserId == userId && t.Id == taskInfo.TaskId);
            if (task == null)
            {
                return BadRequest("Task not exist");
            }
            task.IsFavorite = !task.IsFavorite;
            db.SaveChanges();

            return Ok();
            // AllFavTasks
            //db.Groups.Include(g=>g.Tasks).ToList().ForEach(g=>favTasks.AddRange(g.Tasks.Where(t=>t.IsFavorite == true)));
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetConfirmed([FromBody] SetTask taskInfo)
        {
            if (taskInfo.TaskId == 0)
            {
                return BadRequest("Incorrect parametr");
            }
            var user = db.Users.FirstOrDefault(a => a.Email == HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest("User null");

            int userId = user.Id;
            var task = db.Tasks.Include(t => t.Group.User).FirstOrDefault(t => t.Group.UserId == userId && t.Id == taskInfo.TaskId);
            if (task == null)
            {
                return BadRequest("Task not exist");
            }
            task.IsDone = true;
            task.IsFavorite = false;

            db.SaveChanges();

            return Ok();
        }

    }
}
