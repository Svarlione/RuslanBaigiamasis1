using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;
using RuslanAPI.Services.Authorization;
using RuslanAPI.Services.Mappers;
using RuslanAPI.Services.UserServices;
using System.Net.Mime;
using System.Security.Claims;


/// <summary>
/// Контроллер для управления пользователями в системе регистрации.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserUpdateConteoller : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserMapper _userMapper;
    private readonly IAuthService _authService;

    private readonly long userId;
    public UserUpdateConteoller(IUserService userService, IUserMapper userMapper, IAuthService authService, IHttpContextAccessor accessor)

    {
        _userService = userService;
        _userMapper = userMapper;
        _authService = authService;
        userId = long.Parse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }



    /// <summary>
    /// Обновляет информацию о пользователе.
    /// </summary>
    /// <param name="updateUserDto">Обновленные данные пользователя.</param>
    /// <returns>HTTP-статус 200 в случае успешного обновления или HTTP-статус 400 в случае ошибки.</returns>
    /// 


    [HttpPut("userUpdate")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            _userService.UpdateUser(updateUserDto, userId);
            return Ok(userId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPost("addressCreate")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult CreateUserAddress([FromBody] AdressDto userAddressDto)//rabotaet
    {
        try
        {
            _userService.CreateUserAddress(userAddressDto, userId);
            return Ok();// esli uspeesh vozvrawat' user address id
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPut("addressUpdate")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult UpdateUserAddress([FromBody] AdressDto userAddressDto)
    {
        try
        {
            long updatedAddressId = _userService.UpdateUserAddress(userAddressDto, userId);
            return Ok(new { UpdatedAddressId = updatedAddressId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPost("imageCreate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult CreateImage([FromForm] ImageDto imageDto)
    {
        try
        {
            _userService.CreateImage(imageDto, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }


    [HttpPut("imageUpdate")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult UpdateImage([FromForm] ImageDto imageDto)
    {
        try
        {
            long updateImageId = _userService.UpdateImage(imageDto, userId);
            return Ok(new { UpdateImageId = updateImageId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }



    [HttpGet("user")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult GetUserByUserId()//rabotaet
    {
        try
        {
            var user = _userService.GetUserByUserId(userId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }


    [HttpDelete("userDelete/{userIdToDelete}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [Authorize(Roles = "Administrator")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult DeleteUser(long userIdToDelete)
    {
        try
        {
            _userService.DeleteUser(userIdToDelete, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

}
