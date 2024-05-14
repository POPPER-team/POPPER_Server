using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class Ingredient
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public double Amount { get; set; }

    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
