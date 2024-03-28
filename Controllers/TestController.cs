using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using POPPER_Server.Dtos;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IMongoDatabase _userDatabase;
    private readonly IMinioService _minioService;
    private readonly MySqlConnection _mySqlConnection;

    public TestController(IMongoDatabase userDatabase, IMinioService minioService, MySqlConnection mySqlConnection)
    {
        _userDatabase = userDatabase;
        _minioService = minioService;
        _mySqlConnection = mySqlConnection;
    }

    [HttpGet("[action]")]
    public IActionResult GetUser()
    {
        var collection = _userDatabase.GetCollection<BsonDocument>("testCollection");
        var document = collection.Find(new BsonDocument());
        var dictionary = document.ToList().Select(x => x.ToDictionary());
        return Ok(dictionary);
    }

    [HttpPost("[action]")]
    public IActionResult PostUser(string postString)
    {
        var collection = _userDatabase.GetCollection<BsonDocument>("testCollection");
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

    [HttpGet("[action]")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var filePath = Path.GetTempFileName();
        await _minioService.DownloadFileAsync("test-bucker", fileName, filePath);
        return PhysicalFile(filePath, "application/octet-stream", fileName);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ListFiles()
    {
        return Ok(await _minioService.GetListFilesAsync("test-bucker"));
    }

    [HttpGet("[action]")]
    public async Task<bool> DatabaseList()
    {
        _mySqlConnection.Open();
        return await _mySqlConnection.PingAsync();
    }
}