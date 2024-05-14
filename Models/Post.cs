using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string MediaGuid { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Ingredient> Ingredients { get; } = new List<Ingredient>();

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual ICollection<Saved> Saveds { get; } = new List<Saved>();

    public virtual ICollection<Step> Steps { get; } = new List<Step>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<View> Views { get; } = new List<View>();
}
