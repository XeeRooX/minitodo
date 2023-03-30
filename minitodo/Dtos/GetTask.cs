using System.ComponentModel.DataAnnotations;

namespace minitodo.Dtos
{
    public class GetTask
    {
        [Required]
        public int GroupId { get; set; }
    }
}
