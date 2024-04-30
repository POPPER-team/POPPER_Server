using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class UserDetailsController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;
    private readonly IUserProfileService _UserProfile;

    public UserDetailsController(IUserServices userServices, IMapper mapper, IUserProfileService userProfile)
    {
        _userServices = userServices;
        _mapper = mapper;
        _UserProfile = userProfile;
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetYourData()
    {
        User user = await Request.GetUserAsync();
        return Ok(_mapper.Map<UserDetailsDto>(user));
    }

    [Authorize]
    [HttpGet("[action]/{userGuid}")]
    public async Task<IActionResult> GetUser([FromRoute] string userGuid)
    {
        return Ok(_mapper.Map<UserDetailsDto>(await _userServices.GetUserAsync(userGuid)));
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> SearchUser(string searchString)
    {
        return Ok(_mapper.Map<List<UserDto>>(await _userServices.SearchUserAsync(searchString)));
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] FileUploadDto file)
    {
        User user = await _UserProfile.SetProfilePicture((await Request.GetUserAsync()), file);
        return Ok(user);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> DownloadProfilePicture()
    {
        User user = await Request.GetUserAsync();
        string fileName = await _UserProfile.GetProfilePicture(user);
        var bytes = System.IO.File.ReadAllBytes(fileName);
        FileContentResult file = new FileContentResult(bytes, "image/jpg");
        return file;
    }
}