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
    private readonly IUserProfileService _userProfile;

    public UserDetailsController(IUserServices userServices, IMapper mapper, IUserProfileService userProfile)
    {
        _userServices = userServices;
        _mapper = mapper;
        _userProfile = userProfile;
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
    //TODO looks like the file uplod does not work
    [HttpPut("[action]")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] FileUploadDto file)
    {
        bool success = await _userProfile.SetProfilePicture((await Request.GetUserAsync()), file);
        if (!success) return BadRequest();
        return Ok();
    }
    //TODO return 404 for picture not found
    [HttpGet("[action]")]
    public async Task<IActionResult> DownloadProfilePicture(string? userGuid)
    {
        User user;
        if (userGuid == null)
        {
            user = await Request.GetUserAsync();
        }
        else
        {
            user = await _userServices.GetUserAsync(userGuid);
        }

        if (user == null) return BadRequest();
        return await _userProfile.GetProfilePicture(user);
    }
}
