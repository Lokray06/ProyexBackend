namespace ProyexBackend.Models
{
    public class ProjectPermission
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }

        // Enum from your PostgreSQL type "userRoles"
        public UserRole ProjectRole { get; set; } = UserRole.Worker;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
