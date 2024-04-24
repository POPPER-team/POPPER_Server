using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IMinioService _minioService;
    private readonly ISessionService _session;

    public TestController(IMongoDatabase userDatabase, IMinioService minioService, ISessionService session)
    {
        _minioService = minioService;
        _session = session;
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
    public async Task<IActionResult> GetSession()
    {
        return Ok(await Request.GetSessionAsync());
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> StoreText([FromForm] string text)
    {
        Session s = await Request.GetSessionAsync();
        s.text =text;
        await _session.UpdateText(s,text);
        return Ok(s);
    }
}