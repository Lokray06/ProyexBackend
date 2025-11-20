namespace ProyexBackend.Models;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// User who created/manages the group
    /// </summary>
    public int OwnerId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public GroupType Type { get; set; } = GroupType.Team;

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
