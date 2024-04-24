using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POPPER_Server.Models;
public class Session 
{
    [BsonId]
    public string _Id { get; set; }
    public string SessionGuid { get; set; } = Guid.NewGuid().ToString();

    public string UserGuid { get; set; }
    /*TODO add more datad when we implement posts
    here should be next post to recommend both for following and for you pages
    and any other datata that should presis betwen requests
    */
}