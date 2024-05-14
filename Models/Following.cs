﻿using System;
using System.Collections.Generic;

namespace POPPER_Server.Models;

public partial class Following
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int FollowingId { get; set; }

    public virtual User FollowingNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
