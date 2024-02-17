﻿using Microsoft.AspNetCore.Authorization;
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

    ///// <summary>
    ///// Создает нового пользователя.
    ///// </summary>
    ///// <param name="createUserDto">Данные пользователя для создания.</param>
    ///// <returns>HTTP-статус 200 с идентификатором созданного пользователя или HTTP-статус 400 в случае ошибки.</returns>
    //[HttpPost("user/Create")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
    //{
    //    try
    //    {
    //        _userService.CreateUser(createUserDto);
    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { ErrorMessage = ex.Message });
    //    }
    //}

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
            return Ok();
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
            UserAdress userAddress = _userMapper.MapToUserAdressEntity(userAddressDto);
            _userService.CreateUserAddress(userAddress, userId);
            return Ok(userId);
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

            // Вызываем метод для обновления адреса пользователя
            long updatedAddressId = _userService.UpdateUserAddress(userAddressDto, userId);

            // Возвращаем Id обновленного адреса
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


    [HttpPut("image/update/{id}")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult UpdateImage(int id, [FromForm] ImageUpdateDto imageUpdateDto)
    {
        try
        {
            _userService.UpdateImage(imageUpdateDto, id);
            return Ok();
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


    [HttpDelete("user/delete/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Administrator")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult DeleteUser(long deleteUserId)
    {
        try
        {
            _userService.DeleteUser(deleteUserId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

}
