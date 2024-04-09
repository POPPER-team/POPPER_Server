using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public DateTime? Created { get; set; }

    public string? Status { get; set; }

    public string? WebLink { get; set; }

    public string? PreferedUnits { get; set; }

    public string? Language { get; set; }

    public virtual ICollection<Following> FollowingFollowingNavigations { get; } = new List<Following>();

    public virtual ICollection<Following> FollowingUsers { get; } = new List<Following>();
}
