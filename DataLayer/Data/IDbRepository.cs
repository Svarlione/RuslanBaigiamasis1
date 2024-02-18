using RuslanAPI.Core.Models;

namespace RuslanAPI.DataLayer.Data
{
    public interface IDbRepository
    {
        long Create(User user);
        void CreateAdress(UserAdress userAdress);
        void CreatImage(Image image);
        void DeleteUser(long deleteUserId, long userId);
        UserAdress GetUserAddress(long userId);
        User GetUserByUserId(long userId);
        long UpdateAdress(UserAdress userAdress);
        void UpdateImage(Image image);
        void UpdateUser(User user);


    }
}