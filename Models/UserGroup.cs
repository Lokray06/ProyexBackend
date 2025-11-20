namespace ProyexBackend.Models
{
    public class UserGroup
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
}