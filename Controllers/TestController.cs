using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IMongoDatabase _database;

    public TestController(IMongoDatabase database)
    {
        _database = database;
    }
    // GET
    [HttpGet]
    public IActionResult Get()
    {
        var collection = _database.GetCollection<BsonDocument>("testCollection");
        var document = collection.Find(new BsonDocument());
        var dictionary = document.ToList().Select(x => x.ToDictionary());
        return Ok(dictionary);
    }
    [HttpPost]
    public IActionResult Post(string postString)
    {
        var collection = _database.GetCollection<BsonDocument>("testCollection");
        collection.InsertOne(new BsonDocument("test", postString));
        return Ok();
    }
    [HttpPost("createCollection")]
    public IActionResult CreateCollection()
    {
        _database.CreateCollection("testCollection");
        return Ok($"Collection testCollection created successfully.");
    }
}