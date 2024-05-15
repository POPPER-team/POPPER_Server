using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class View
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
