using System;
using System.Collections.Generic;

namespace ProyexBackend.Models;

public partial class UserProject
{
    public int UserId { get; set; }

    public int ProjectId { get; set; }

    public DateTime? JoinDate { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
