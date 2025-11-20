using System;
using System.Collections.Generic;

namespace ProyexBackend.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
