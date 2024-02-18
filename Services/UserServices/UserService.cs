using Microsoft.AspNetCore.Http;
using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;
using RuslanAPI.DataLayer.Data;
using RuslanAPI.Services.Mappers;

namespace RuslanAPI.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IDbRepository _dbRepository;
        private readonly IUserMapper _userMapper;

        public UserService(IDbRepository dbRepository, IUserMapper userMapper)
        {
            _dbRepository = dbRepository;
            _userMapper = userMapper;
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
                // Здесь можно добавить логирование ошибки или обработку исключения
                throw new InvalidOperationException("An error occurred while creating the user.", ex);
            }
        }

        public void UpdateUser(UpdateUserDto updateUserDto, long userId)
        {
            User user = _userMapper.MapToUserEntity(updateUserDto);
            _dbRepository.UpdateUser(user);
        }

        //public void DeleteUser(long deleteUserId, long userId)
        //{
        //    if ()
        //        _dbRepository.DeleteUser(deleteUserId);
        //}

        public User GetUserByUserId(long userId)
        {
            return _dbRepository.GetUserByUserId(userId);
        }

        public void CreateUserAddress(UserAdress userAddress, long userid)
        {
            if (userAddress.Id == 0)
            {
                userAddress.UserId = userid;
                _dbRepository.CreateAdress(userAddress);
            }
            else
            {
                throw new InvalidOperationException("Cannot create user address with an existing ID.");
            }
        }

        public long UpdateUserAddress(AdressDto userAddressDto, long userid)
        {
            if (userid != 0)
            {
                var userAddressFromDb = _dbRepository.GetUserAddress(userid);
                if (userAddressFromDb == null)
                    throw new InvalidOperationException("exception.");

                var userAddress = new UserAdress()
                {
                    Town = userAddressDto.Town,
                    Road = userAddressDto.Road,
                    HomeNumer = userAddressDto.HomeNumer,
                    FlatNumber = userAddressDto.FlatNumber,
                    Type = userAddressDto.Type,
                    Id = userAddressFromDb.Id,
                    UserId = userid
                };

                return _dbRepository.UpdateAdress(userAddress);
            }
            else
            {
                throw new InvalidOperationException("Cannot update user address with zero ID.");
            }
        }

        public void CreateImage(ImageDto imageDto, long userId)
        {
            try
            {
                Image image = _userMapper.MapToImageEntity(imageDto);
                image.UserId = userId;
                _dbRepository.CreatImage(image);
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибки или обработку исключения
                throw new InvalidOperationException("An error occurred while creating the image.", ex);
            }
        }

        public long UpdateImage(ImageUpdateDto imageUpdateDto, long userid)
        {
            if (userid != 0)
            {
                var userImageFromDb = _dbRepository.GetUserImage(userid);
                if (userImageFromDb == null)
                    throw new InvalidOperationException("exception.");

                var image = new Image()
                {
                    Description = imageUpdateDto.Description,
                    Name = imageUpdateDto.Name,
                    ImageBytes = imageUpdateDto.Image != null ?
                                 GetImageBytes(imageUpdateDto.Image) : null,
                    Id = userImageFromDb.Id,
                    UserId = userid
                };

                _dbRepository.UpdateImage(image);
                return userImageFromDb.Id;
            }
            else
            {
                throw new InvalidOperationException("Cannot update image with zero ID.");
            }
        }

        private byte[] GetImageBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
