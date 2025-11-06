namespace ProyexBackend.Models
{
    public enum UserRole
    {
        Administrator,
        ProjectManager,
        Worker,
        Collaborator,
        Contributor
    }

    public enum GroupType
    {
        Team,
        Organization,
        Committee,
        PersonalGroup
    }

    public enum TaskStatus
    {
        PendingAssignment,
        InProgress,
        Blocked,
        Completed
    }

    public enum ProjectStatus
    {
        Active,
        Archived,
        OnHold
    }

    public enum Priority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum Visibility
    {
        Public,
        Private
    }
}
