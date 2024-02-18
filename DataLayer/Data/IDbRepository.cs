using RuslanAPI.Core.Models;

namespace RuslanAPI.DataLayer.Data
{
    public interface IDbRepository
    {
        long Create(User user);
        void CreateAdress(UserAdress userAdress);
        void CreatImage(Image image);
        void DeleteUser(User userToDelete);
        UserAdress GetUserAddress(long userId);
        User GetUserByUserId(long userId);
        long UpdateAdress(UserAdress userAdress);
        long UpdateImage(Image image);
        void UpdateUser(User user);

        Image GetUserImage(long userId);
    }
}