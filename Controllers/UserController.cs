using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public UserController(IUserServices userServices, IMapper mapper)
    {
        _userServices = userServices;
        _mapper = mapper;
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Login([FromHeader]string username, [FromHeader]string password)
    {
        return Ok(await _userServices.LoginUserAsync(username, password));
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromForm] UserDto userDto)
    {
        User newUser = _mapper.Map<UserDto, User>(userDto);
        await _userServices.RegisterUserAsync(newUser); 
        return Ok();
    }
    [HttpGet("[action]/{userGuid}")]
    public async Task<IActionResult> GetUser([FromRoute] string userGuid)
    {
        return Ok(_mapper.Map<UserDto>(await _userServices.GetUserAsync(userGuid)));
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> SearchUser(string searchString)
    {
        return Ok(_mapper.Map<List<UserDto>>(await _userServices.SearchUserAsync(searchString)));
    }
}