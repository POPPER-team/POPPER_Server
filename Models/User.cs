﻿using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string Guid { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Created { get; set; }

    public string? Status { get; set; }

    public string? WebLink { get; set; }

    public string? PreferredUnits { get; set; }

    public string? Language { get; set; }

    public virtual ICollection<Following> FollowingFollowingNavigations { get; } = new List<Following>();

    public virtual ICollection<Following> FollowingUsers { get; } = new List<Following>();
}
