using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;

namespace RuslanAPI.Services.Mappers
{
    public interface IUserMapper
    {
        Image MapToImageEntity(ImageDto imageDto, long userId);
        UserAdress MapToUserAdressEntity(AdressDto addressDto, long userId);
        User MapToUserEntity(CreateUserDto createUserDto);
        User MapToUserEntity(UpdateUserDto updateUserDto);
    }
}