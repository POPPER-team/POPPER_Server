using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    //TODO split into two controllers UserAuthenticationController and UserDetailsController
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public UserController(IUserServices userServices, IMapper mapper)
    {
        _userServices = userServices;
        _mapper = mapper;
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Login([FromHeader]UserLoginDto userLoginDto)
    {
        return Ok(await _userServices.LoginUserAsync(userLoginDto.Username, userLoginDto.Password));
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromForm] NewUserDto userDto)
    {
        await _userServices.RegisterUserAsync(userDto); 
        return Ok();
    }
    [HttpGet("[action]/{userGuid}")]
    public async Task<IActionResult> GetUser([FromRoute] string userGuid)
    {
        return Ok(_mapper.Map<UserDetailsDto>(await _userServices.GetUserAsync(userGuid)));
    }
    //TODO remove when we add more Authorized enpoints Now it is used for testing
    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> SearchUser(string searchString)
    {
        return Ok(_mapper.Map<List<UserDto>>(await _userServices.SearchUserAsync(searchString)));
    }
}