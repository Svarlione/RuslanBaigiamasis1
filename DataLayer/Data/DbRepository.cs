using Microsoft.EntityFrameworkCore;
using RuslanAPI.Core.DTO;
using RuslanAPI.Core.Models;
using Image = RuslanAPI.Core.Models.Image;

namespace RuslanAPI.DataLayer.Data
{
    public class DbRepository : IDbRepository
    {
        private readonly UserDbContext _userDbContext;

        public DbRepository(UserDbContext context)
        {
            _userDbContext = context;
        }

        /// <summary>
        /// Kuriam nauja Useri.
        /// </summary>
        /// <param name="user">.</param>

        public long Create(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");

                _userDbContext.Users.Add(user);
                _userDbContext.SaveChanges();

                if (user.Id <= 0)
                    throw new InvalidOperationException("User ID is invalid after creation.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while saving the entity changes.", ex);
            }
            return user.Id;

        }


        /// <summary>
        /// Atnaujinam useri.
        /// </summary>
        /// <param name="user">.</param>
        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Nothing to update.");

            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// Trina Useri pagal Id.
        /// </summary>
        /// <param name="userIdToDelete">Trinamojo Userio Id.</param>
        public void DeleteUser(User userToDelete)
        {
            _userDbContext.Users.Remove(userToDelete);
            _userDbContext.SaveChanges();
        }


        /// <summary>
        /// Gaunam visa info Userio pagal Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Grazina info apie Useri</returns>
        public User GetUserByUserId(long userId)
        {
            return _userDbContext.Users
                                       .Include(u => u.Image)
                                       .Include(u => u.Adress)
                                       .Include(u => u.LoginInfo)
                                       .FirstOrDefault(u => u.Id == userId);
        }

        public UserAdress GetUserAddress(long userId)
        {
            return _userDbContext.UserAdress.FirstOrDefault(x => x.UserId == userId);
        }

        /// <summary>
        /// Sukuria nauja adresa.
        /// </summary>
        /// <param name="userAdress">.</param>
        public void CreateAdress(UserAdress userAdress)
        {
            if (userAdress == null)
                throw new ArgumentNullException(nameof(userAdress), "User address cannot be null.");

            _userDbContext.UserAdress.Add(userAdress);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// Atnaujina Usrio adresa
        /// </summary>
        /// <param name="userAdress">atnaujina duomenis.</param>
        public long UpdateAdress(UserAdress userAdress)
        {
            UserAdress existingAddress = _userDbContext.UserAdress.FirstOrDefault(x => x.UserId == userAdress.UserId);
            if (existingAddress != null)
            {
                _userDbContext.Entry(existingAddress).CurrentValues.SetValues(userAdress);
                _userDbContext.SaveChanges();

                return existingAddress.Id;
            }
            else
            {
                throw new InvalidOperationException("User address not found for the given userId.");
            }
        }



        /// <summary>
        /// Sukuria Image.
        /// </summary>
        /// <param name="image">.</param>
        public void CreatImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "No image to create.");

            _userDbContext.Image.Add(image);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// atnaujina Image.
        /// </summary>
        /// <param name="image">Atnaujina duomenis.</param>
        public long UpdateImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Nothing to update.");

            _userDbContext.Image.Update(image);
            _userDbContext.SaveChanges();
            return image.Id;
        }

        public Image GetUserImage(long userId)
        {
            return _userDbContext.Image.AsNoTracking().FirstOrDefault(i => i.UserId == userId);
        }

        public void DetachEntity(object entity)
        {
            var entry = _userDbContext.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Detached;
            }
        }


    }
}
