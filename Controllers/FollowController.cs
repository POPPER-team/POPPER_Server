using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    private readonly IMapper _mapper;

    public FollowController(IFollowService followService, IMapper mapper)
    {
        _followService = followService;
        _mapper = mapper;
    }

    [HttpPost("[action]/{followingGuid}")]
    public async Task<IActionResult> FollowUser([FromRoute] string followingGuid)
    {
        User user = await Request.GetUserAsync();
        await _followService.FollowUserAsync(user, followingGuid);
        return Ok();
    }

    [HttpDelete("[action]/{followingGuid}")]
    public async Task<IActionResult> UnFollowUser([FromRoute] string followingGuid)
    {
        User user = await Request.GetUserAsync();
        try
        {
            await _followService.UnFollowUserAsync(user, followingGuid);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetFollowers()
    {
        User user = await Request.GetUserAsync();
        return Ok(_mapper.Map<List<UserDto>>(await _followService.GetFollowersAsync(user)));
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetFollowing()
    {
        User user = await Request.GetUserAsync();
        return Ok(_mapper.Map<List<UserDto>>(await _followService.GetFollowingAsync(user)));
    }
}