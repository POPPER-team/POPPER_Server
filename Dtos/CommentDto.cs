namespace POPPER_Server.Dtos;

public class CommentDto
{
    public string Guid { get; set; }
    public UserDto UserDto { get; set; }
    public string Text { get; set; }
    public int Rating { get; set; }
}