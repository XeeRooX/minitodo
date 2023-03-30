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

        [HttpPost]
        public IActionResult Create([FromBody] TaskCreate task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad content");
            }

            int userId = 1;
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

        [HttpDelete]
        public IActionResult Delete([FromBody] TaskDelete taskInfo)
        {
            if (!ModelState.IsValid || taskInfo.TaskId == 0)
            {
                return BadRequest("Bad content");
            }

            int userId = 1;
            //var task = db.Tasks.Find(taskInfo.TaskId);
            var task = db.Tasks.Include(t => t.Group).FirstOrDefault(t => t.Group.UserId == userId && t.Id == taskInfo.TaskId);
            
            if (task == null)
                return BadRequest("Not exist");

            db.Tasks.Remove(task);
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult Edit([FromBody] TaskEdit taskInfo)
        {
            if (!ModelState.IsValid || taskInfo.TaskId == 0)
            {
                return BadRequest("Bad content");
            }

            int userId = 1;
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

        [HttpGet]
        public IActionResult GetAll([FromBody] GetTask groupInfo)
        {
            if (groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            int userId = 1;
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

        [HttpPost]
        public IActionResult GetConfirmed([FromBody] GetTask groupInfo)
        {
            if (groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            int userId = 1;
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

        [HttpPost]
        public IActionResult GetNotConfirmed([FromBody] GetTask groupInfo)
        {
            if (groupInfo == null || groupInfo.GroupId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            int userId = 1;

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

        [HttpGet]
        public IActionResult GetFavorited()
        {
            int userId = 1;

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

        [HttpPost]
        public IActionResult SetFavorited([FromBody]SetTask taskInfo)
        {
            if (taskInfo.TaskId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            // Пока что
            int userId = 1;

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

        [HttpPost]
        public IActionResult SetConfirmed([FromBody] SetTask taskInfo)
        {
            if (taskInfo.TaskId == 0)
            {
                return BadRequest("Incorrect parametr");
            }

            int userId = 1;
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
