using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
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

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromQuery] UserLoginDto userLoginDto)
    {
        try
        {
            TokensDto tokens = await _userServices.LoginUserAsync(userLoginDto.Username, userLoginDto.Password);
            return Ok(tokens);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] NewUserDto userDto)
    {
        await _userServices.RegisterUserAsync(userDto);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        throw new NotImplementedException();
        var newJwtToken = await _userServices.RefreshJwtTokenAsync(refreshToken);
        return Ok(newJwtToken);
    }
}