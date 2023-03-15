namespace minitodo.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public List<Task> Tasks { get; set; } = new();
    }
}
