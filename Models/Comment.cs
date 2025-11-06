using System;
using System.Collections.Generic;

namespace ProyexBackend.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    /// <summary>
    /// Comment belongs to a task
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// User who made the comment
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Self-reference for thread replies. NULL for top-level comments.
    /// </summary>
    public int? ParentCommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
