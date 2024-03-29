﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;
using RuslanAPI.DataLayer.Data;
using RuslanAPI.Services.Authorization;
using RuslanAPI.Services.Mappers;

namespace RuslanAPI.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IDbRepository _dbRepository;
        private readonly IUserMapper _userMapper;
        private readonly IAuthService _authService;

        public UserService(IDbRepository dbRepository, IUserMapper userMapper, IAuthService authService)
        {
            _dbRepository = dbRepository;
            _userMapper = userMapper;
            _authService = authService;
        }

        public long CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                User user = _userMapper.MapToUserEntity(createUserDto);
                return _dbRepository.Create(user);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("Something goes wrong while creating the user.", ex);
            }
        }

        public void UpdateUser(UpdateUserDto updateUserDto, long userId)
        {
            var user = _userMapper.MapToUserEntity(updateUserDto); // useris is mapperio
            var userFromDb = _dbRepository.GetUserByUserId(userId); // Useris is domenu bazes

            //uploudinam senaja info, kad ne atnaujinti visko ir ne mestu klaidos is entities
            user.PersonalIndefication = userFromDb.PersonalIndefication;
            user.Id = userFromDb.Id;
            user.Adress = userFromDb.Adress;
            user.Image = userFromDb.Image;


            user.LoginInfo = new LoginInfo
            {
                Id = userFromDb.LoginInfo.Id,
                Role = userFromDb.LoginInfo.Role,
                PasswordSalt = userFromDb.LoginInfo.PasswordSalt,
                UserName = userFromDb.LoginInfo.UserName,
                Password = _authService.HashPassword(updateUserDto.Password)
            };

            // atjungiam objekta nuo kito objekto kad galetu atsinaujinti
            _dbRepository.DetachEntity(userFromDb);


            _dbRepository.UpdateUser(user);
        }


        public void DeleteUser(long userIdToDelete, long userId)
        {
            var deletingUser = _dbRepository.GetUserByUserId(userId);
            var userToDelete = _dbRepository.GetUserByUserId(userIdToDelete);

            if (deletingUser != null && userToDelete != null && deletingUser.LoginInfo.Role == "Administrator")
                _dbRepository.DeleteUser(userToDelete);
            else
                throw new InvalidOperationException("Cant delete user, you dont have promision or User dont exist.");
        }

        public User GetUserByUserId(long userId)
        {
            return _dbRepository.GetUserByUserId(userId);
        }

        public void CreateUserAddress(AdressDto adressDto, long userid)
        {
            try
            {
                UserAdress userAddress = _userMapper.MapToUserAdressEntity(adressDto, userid);
                _dbRepository.CreateAdress(userAddress);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("error");// change exption
            }
        }

        public long UpdateUserAddress(AdressDto addressDto, long userid)
        {
            try
            {
                var userAddressFromDb = _dbRepository.GetUserAddress(userid);
                if (userAddressFromDb == null)
                    throw new InvalidOperationException("exception.");

                UserAdress userAddress = _userMapper.MapToUserAdressEntity(addressDto, userid);
                userAddress.Id = userAddressFromDb.Id;

                return _dbRepository.UpdateAdress(userAddress);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("No address to update");
            }
        }

        public void CreateImage(ImageDto imageDto, long userId)
        {
            try
            {
                Image image = _userMapper.MapToImageEntity(imageDto, userId);
                _dbRepository.CreatImage(image);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while creating the image.", ex);
            }
        }

        public long UpdateImage(ImageDto imageDto, long userid)
        {
            if (userid != 0)
            {
                var userImageFromDb = _dbRepository.GetUserImage(userid);
                if (userImageFromDb == null)
                    throw new InvalidOperationException("exception.");

                var userImage = _userMapper.MapToImageEntity(imageDto, userid);
                userImage.Id = userImageFromDb.Id;

                _dbRepository.UpdateImage(userImage);
                return userImageFromDb.Id;
            }
            else
            {
                throw new InvalidOperationException("Cannot update image with zero ID.");
            }
        }

    }
}
