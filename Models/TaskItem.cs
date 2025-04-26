using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int? AssignedUserId { get; set; }
        public User User { get; set; } 
    }

}
