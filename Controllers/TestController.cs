using Microsoft.AspNetCore.Mvc;
using Minio;
using MongoDB.Bson;
using MongoDB.Driver;
using POPPER_Server.Dtos;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly IMinioService  _minioService;
    public TestController(IMongoDatabase database, IMinioService minioService)
    {
        _database = database;
        _minioService = minioService;
    }
    // GET
    [HttpGet("[action]")]
    public IActionResult GetUser()
    {
        var collection = _database.GetCollection<BsonDocument>("testCollection");
        var document = collection.Find(new BsonDocument());
        var dictionary = document.ToList().Select(x => x.ToDictionary());
        return Ok(dictionary);
    }
    [HttpPost("[action]")]
    public IActionResult PostUser(string postString)
    {
        var collection = _database.GetCollection<BsonDocument>("testCollection");
        collection.InsertOne(new BsonDocument("test", postString));
        return Ok();
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadDto file)
    {
        
        var filePath = Path.GetTempFileName();

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.File.CopyToAsync(stream);
        }

        await _minioService.UploadFileAsync("test-bucker", file.File.FileName, filePath);

        return Ok($"File {file.File.FileName} uploaded successfully.");
    }
}