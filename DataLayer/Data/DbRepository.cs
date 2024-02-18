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
        /// Создает нового пользователя.
        /// </summary>
        /// <param name="user">Данные пользователя для создания.</param>
        /// <returns>Идентификатор созданного пользователя.</returns>
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
                // Здесь можно добавить логирование ошибки или обработку исключения
                throw new InvalidOperationException("An error occurred while saving the entity changes.", ex);
            }
            return user.Id;

        }


        /// <summary>
        /// Обновляет информацию о пользователе.
        /// </summary>
        /// <param name="user">Обновленные данные пользователя.</param>
        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// Удаляет пользователя по его идентификатору.
        /// </summary>
        /// <param name="userIdToDelete">Идентификатор пользователя для удаления.</param>
        public void DeleteUser(User userToDelete)
        {
            _userDbContext.Users.Remove(userToDelete);
            _userDbContext.SaveChanges();
        }


        /// <summary>
        /// Получает пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Найденный пользователь.</returns>
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
        /// Создает новый адрес пользователя.
        /// </summary>
        /// <param name="userAdress">Данные адреса для создания.</param>
        public void CreateAdress(UserAdress userAdress)
        {
            if (userAdress == null)
                throw new ArgumentNullException(nameof(userAdress), "User address cannot be null.");

            _userDbContext.UserAdress.Add(userAdress);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// Обновляет информацию о пользовательском адресе.
        /// </summary>
        /// <param name="userAdress">Обновленные данные пользовательского адреса.</param>
        public long UpdateAdress(UserAdress userAdress)
        {
            // Получаем существующий адрес по userId
            UserAdress existingAddress = _userDbContext.UserAdress.FirstOrDefault(x => x.UserId == userAdress.UserId);
            if (existingAddress != null)
            {
                // Обновляем значения свойств в существующем адресе
                _userDbContext.Entry(existingAddress).CurrentValues.SetValues(userAdress);

                // Сохраняем изменения в базе данных
                _userDbContext.SaveChanges();

                // Возвращаем Id обновленного адреса
                return existingAddress.Id;
            }
            else
            {
                // Обработка ситуации, когда адрес не найден
                throw new InvalidOperationException("User address not found for the given userId.");
            }
        }



        /// <summary>
        /// Создает новое изображение.
        /// </summary>
        /// <param name="image">Данные изображения для создания.</param>
        public void CreatImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");

            _userDbContext.Image.Add(image);
            _userDbContext.SaveChanges();
        }

        /// <summary>
        /// Обновляет информацию об изображении.
        /// </summary>
        /// <param name="image">Обновленные данные изображения.</param>
        public long UpdateImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");

            _userDbContext.Image.Update(image);
            _userDbContext.SaveChanges();
            return image.Id;
        }

        public Image GetUserImage(long userId)
        {
            return _userDbContext.Image.AsNoTracking().FirstOrDefault(i => i.UserId == userId);
        }
    }
}
