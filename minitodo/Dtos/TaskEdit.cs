using System.ComponentModel.DataAnnotations;

namespace minitodo.Dtos
{
    public class TaskEdit
    {
        [Required]
        public int TaskId { get; set; }
        [Required]
        public string TaskTitle { get; set; }
    }
}
