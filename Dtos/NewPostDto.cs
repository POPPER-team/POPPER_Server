namespace POPPER_Server.Dtos;

public class NewPostDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile Media { get; set; }
    public List<string> Ingridients { get; set; }
    public List<string> Steps { get; set; }
}
