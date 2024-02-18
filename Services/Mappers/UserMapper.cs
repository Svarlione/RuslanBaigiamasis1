﻿using Microsoft.AspNetCore.Http;
using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RuslanAPI.Services.Mappers
{
    public class UserMapper : IUserMapper
    {
        public User MapToUserEntity(CreateUserDto createUserDto)
        {
            return new User()
            {

                PersonalIndefication = createUserDto.PersonalIndefication,
                Email = createUserDto.Email,

                LoginInfo = new LoginInfo()
                {
                    UserName = "",
                    Password = new byte[] { },
                    PasswordSalt = new byte[] { },
                    Role = "user"
                },
            };
        }

        public User MapToUserEntity(UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {
                return null;
            }

            return new User()
            {
                Email = updateUserDto.Email,
                PhoneNumber = updateUserDto.PhoneNumber
            };
        }

        public UserAdress MapToUserAdressEntity(AdressDto addressDto, long userId)
        {
            if (addressDto == null)
            {
                return null;
            }

            // Проверка обязательных полей
            var context = new ValidationContext(addressDto, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(addressDto, context, results, validateAllProperties: true))
            {
                // Если есть ошибки валидации, вы можете обработать их здесь или выбросить исключение
                throw new ArgumentException("Validation failed for AdressDto", nameof(addressDto));
            }

            return new UserAdress()
            {
                Town = addressDto.Town,
                Road = addressDto.Road,
                HomeNumer = addressDto.HomeNumer,
                FlatNumber = addressDto.FlatNumber,
                Type = addressDto.Type,
                UserId = userId
            };
        }

        public Image MapToImageEntity(ImageDto imageDto, long userId)
        {
            if (imageDto == null)
            {
                return null;
            }

            return new Image()
            {
                Name = imageDto.Name,
                Description = imageDto.Description,
                ImageBytes = ConvertToBytes(imageDto.Image),
                UserId = userId
            };
        }

        //public Image MapToImageEntity(ImageUpdateDto imageUpdateDto, long userId)
        //{
        //    if (imageUpdateDto == null || imageUpdateDto.Image == null)
        //    {
        //        return null;
        //    }

        //    return new Image()
        //    {
        //        Name = imageUpdateDto.Name,
        //        Description = imageUpdateDto.Description,
        //        ImageBytes = ConvertToBytes(imageUpdateDto.Image),
        //        UserId = userId

        //    };
        //}
        private byte[] ConvertToBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
