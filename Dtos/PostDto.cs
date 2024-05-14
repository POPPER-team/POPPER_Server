using POPPER_Server.Dtos;

namespace POPPER_Server.Helpers;

public class PostDto
{
    public string Guid { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }

    public string MediaGuid { get; set; }

    /// <summary>
    /// UTC string representing time
    /// </summary>
    public string Duration { get; set; }

    public string UserGuid { get; set; }

    public int Likes { get; set; }

    public int SavedCount { get; set; }
    public int viewCount { get; set; }
    public List<CommentDto> Comments { get; set; }

    public List<IngridientDto> Ingredients { get; set; }

    public List<StepDto> Steps { get; set; }
}