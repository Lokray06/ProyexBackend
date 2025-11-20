using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // <--- ADD THIS

namespace ProyexBackend.Models;

public partial class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; } = UserRole.Worker;
    public DateTime? CreatedAt { get; set; }
    public byte[]? Avatar { get; set; }
    public DateTime? LastLogin { get; set; }

    // --- ADD [JsonIgnore] TO ALL THESE ---
    // This hides them from the API Swagger documentation and the Request Body

    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [JsonIgnore]
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    [JsonIgnore]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [JsonIgnore]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    [JsonIgnore]
    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();

    [JsonIgnore]
    public virtual ICollection<Group> GroupsNavigation { get; set; } = new List<Group>();

    [JsonIgnore]
    public virtual ICollection<ProjectPermission> ProjectPermissions { get; set; } = new List<ProjectPermission>();
}