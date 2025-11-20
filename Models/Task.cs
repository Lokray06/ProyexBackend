namespace ProyexBackend.Models
{
    public partial class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }

        // Enum properties
        public TaskStatus Status { get; set; } = TaskStatus.PendingAssignment;
        public Priority Priority { get; set; } = Priority.Medium;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();


        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual Project Project { get; set; } = null!;
        public virtual User? AssignedToUser { get; set; }
    }
}