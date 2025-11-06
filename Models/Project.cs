namespace ProyexBackend.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }

        // Enum properties
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public Priority Priority { get; set; } = Priority.Medium;
        public Visibility Visibility { get; set; } = Visibility.Public;

        public int OwnerId { get; set; }
        public int? OwningGroupId { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Navigation
        public virtual User Owner { get; set; } = null!;
        public virtual Group? OwningGroup { get; set; }
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        // Many-to-many navigation
        public virtual ICollection<ProjectPermission> ProjectPermissions { get; set; } = new List<ProjectPermission>();
        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}