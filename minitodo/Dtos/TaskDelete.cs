using System.ComponentModel.DataAnnotations;

namespace minitodo.Dtos
{
    public class TaskDelete
    {
        [Required]
        public int TaskId { get; set; }
    }
}
