using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class Step
{
    public int Id { get; set; }

    public int Step1 { get; set; }

    public string Text { get; set; } = null!;

    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
