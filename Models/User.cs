using System;
using System.Collections.Generic;

namespace ProyexBackend.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>
    /// Store securely
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    // Missing enum
    public UserRole Role { get; set; } = UserRole.Worker; // ← Add this line

    public DateTime? CreatedAt { get; set; }

    public byte[]? Avatar { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();

    public virtual ICollection<Group> GroupsNavigation { get; set; } = new List<Group>();

    public virtual ICollection<ProjectPermission> ProjectPermissions { get; set; } = new List<ProjectPermission>();
}
