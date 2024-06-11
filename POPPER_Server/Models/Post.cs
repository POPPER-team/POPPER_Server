using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Guid { get; set; } = System.Guid.NewGuid().ToString();

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? MediaGuid { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    public int UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Ingredient> Ingredients { get; } = new List<Ingredient>();

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual ICollection<Saved> Saveds { get; } = new List<Saved>();

    public virtual ICollection<Step> Steps { get; } = new List<Step>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<View> Views { get; } = new List<View>();
}
