using Microsoft.EntityFrameworkCore;
using ProyexBackend.Models;


namespace ProyexBackend.Data;

public partial class ProyexDBContext : DbContext
{
    public ProyexDBContext()
    {
    }

    public ProyexDBContext(DbContextOptions<ProyexDBContext> options) : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProject> UserProjects { get; set; }

    public virtual DbSet<ProjectPermission> ProjectPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("groupType", new[] { "team", "organization", "committee", "personalGroup" })
            .HasPostgresEnum("priority", new[] { "low", "medium", "high", "critical" })
            .HasPostgresEnum("projectStatus", new[] { "active", "archived", "onHold" })
            .HasPostgresEnum("taskStatus", new[] { "pendingAssignment", "inProgress", "blocked", "completed" })
            .HasPostgresEnum("userRoles", new[] { "administrator", "projectManager", "worker", "collaborator", "contributor" })
            .HasPostgresEnum("visibility", new[] { "public", "private" });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ParentCommentId)
                .HasComment("Self-reference for thread replies. NULL for top-level comments.")
                .HasColumnName("parentCommentId");
            entity.Property(e => e.TaskId)
                .HasComment("Comment belongs to a task")
                .HasColumnName("taskId");
            entity.Property(e => e.UserId)
                .HasComment("User who made the comment")
                .HasColumnName("userId");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("comments_parentCommentId_fkey");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_taskId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_userId_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.OwnerId)
                .HasComment("User who created/manages the group")
                .HasColumnName("ownerId");

            entity.HasOne(d => d.Owner).WithMany(p => p.Groups)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("groups_ownerId_fkey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_pkey");

            entity.ToTable("projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("dueDate");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.OwnerId)
                .HasComment("Individual user who created the project")
                .HasColumnName("ownerId");
            entity.Property(e => e.OwningGroupId)
                .HasComment("The organization or team officially sponsoring/owning the project")
                .HasColumnName("owningGroupId");
            entity.Property(e => e.StartDate).HasColumnName("startDate");

            entity.HasOne(d => d.Owner).WithMany(p => p.Projects)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projects_ownerId_fkey");

            entity.HasOne(d => d.OwningGroup).WithMany(p => p.Projects)
                .HasForeignKey(d => d.OwningGroupId)
                .HasConstraintName("projects_owningGroupId_fkey");
        });

        modelBuilder.Entity<ProjectPermission>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProjectId }).HasName("projectPermissions_pkey");
            entity.ToTable("projectPermissions");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.ProjectId).HasColumnName("projectId");
            entity.Property(e => e.ProjectRole)
                .HasConversion<string>()
                .HasColumnName("projectRole")
                .HasDefaultValue(UserRole.Worker);

            entity.HasOne(d => d.User)
                .WithMany(p => p.ProjectPermissions) // <-- Point to the new property in User.cs
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projectPermissions_userId_fkey");

            entity.HasOne(d => d.Project)
                .WithMany(p => p.ProjectPermissions) // <-- Point to the new property in Project.cs
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projectPermissions_projectId_fkey");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "tags_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProyexBackend.Models.Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedToUserId)
                .HasComment("Can be NULL if status is pendingAssignment")
                .HasColumnName("assignedToUserId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("dueDate");
            entity.Property(e => e.ProjectId).HasColumnName("projectId");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssignedToUserId)
                .HasConstraintName("tasks_assignedToUserId_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasks_projectId_fkey");

            entity.HasMany(d => d.Tags).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("taskTags_tagId_fkey"),
                    l => l.HasOne<ProyexBackend.Models.Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("taskTags_taskId_fkey"),
                    j =>
                    {
                        j.HasKey("TaskId", "TagId").HasName("taskTags_pkey");
                        j.ToTable("taskTags");
                        j.IndexerProperty<int>("TaskId").HasColumnName("taskId");
                        j.IndexerProperty<int>("TagId").HasColumnName("tagId");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("firstName");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastLogin");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("lastName");
            entity.Property(e => e.PasswordHash)
                .HasComment("Store securely")
                .HasColumnType("character varying")
                .HasColumnName("passwordHash");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");

            entity.HasMany(d => d.GroupsNavigation).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserGroup",
                    r => r.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userGroups_groupId_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userGroups_userId_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "GroupId").HasName("userGroups_pkey");
                        j.ToTable("userGroups");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("GroupId").HasColumnName("groupId");
                    });
        });

        modelBuilder.Entity<UserProject>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProjectId }).HasName("userProjects_pkey");

            entity.ToTable("userProjects");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.ProjectId).HasColumnName("projectId");
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("joinDate");

            entity.HasOne(d => d.Project).WithMany(p => p.UserProjects)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userProjects_projectId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserProjects)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userProjects_userId_fkey");
        });

        // Enum conversions
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasColumnName("role");

        modelBuilder.Entity<Group>()
            .Property(g => g.Type)
            .HasConversion<string>()
            .HasColumnName("type");

        modelBuilder.Entity<Project>()
            .Property(p => p.Status)
            .HasConversion<string>()
            .HasColumnName("status");

        modelBuilder.Entity<Project>()
            .Property(p => p.Priority)
            .HasConversion<string>()
            .HasColumnName("priority");

        modelBuilder.Entity<Project>()
            .Property(p => p.Visibility)
            .HasConversion<string>()
            .HasColumnName("visibility");

        modelBuilder.Entity<ProyexBackend.Models.Task>()
            .Property(t => t.Status)
            .HasConversion<string>()
            .HasColumnName("status");

        modelBuilder.Entity<ProyexBackend.Models.Task>()
            .Property(t => t.Priority)
            .HasConversion<string>()
            .HasColumnName("priority");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
