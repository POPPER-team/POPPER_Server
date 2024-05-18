namespace POPPER_Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string Guid { get; set; } = System.Guid.NewGuid().ToString();

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime? DateOfBirth { get; set; }

    public string? Status { get; set; }

    public string? WebLink { get; set; }

    public string? PreferredUnits { get; set; }

    public string? Language { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Following> FollowingFollowingNavigations { get; } = new List<Following>();

    public virtual ICollection<Following> FollowingUsers { get; } = new List<Following>();

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual ICollection<Saved> Saveds { get; } = new List<Saved>();

    public virtual ICollection<View> Views { get; } = new List<View>();
}