using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class UserAuthenticationController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public UserAuthenticationController(IUserServices userServices, IMapper mapper)
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

}