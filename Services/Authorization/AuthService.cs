﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RuslanAPI.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using RuslanAPI.DataLayer.Data;
using RuslanAPI.Core.DTO;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace RuslanAPI.Services.Authorization
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserDbContext _dbContext;

        public AuthService(IConfiguration configuration, IPasswordHasher<User> passwordHasher, UserDbContext dbContext)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _dbContext = dbContext;
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            try
            {
                var user = await FindUserByNameAsync(userName);

                if (user == null || !VerifyPassword(user.LoginInfo, password))
                {
                    Console.WriteLine($"Invalid username or password for user: {userName}");
                    throw new InvalidOperationException("Invalid username or password.");
                }

                var role = "user";
                var token = GenerateJwtToken(user.LoginInfo.UserName, role, user.Id);

                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex}");
                throw;
            }
        }

        public async Task<string> SignUpAsync(string username, string role, string password, byte[] passwordSalt, string personalIndefication, string email)
        {
            try
            {
                var loginInfo = new LoginInfo
                {
                    UserName = username,
                    Password = HashPassword(password),
                    PasswordSalt = passwordSalt,
                    Role = role
                };

                var moreInfo = new SingUpDto
                {
                    PersonalIndefication = personalIndefication,
                    Email = email,
                };

                var user = CreateUser(username, password, passwordSalt, personalIndefication, email);
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return username;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sign up: {ex}");
                throw;
            }
        }

        private User CreateUser(string username, string password, byte[] passwordSalt, string personalIndefication, string email)
        {
            var loginInfo = new LoginInfo
            {
                UserName = username,
                Password = HashPassword(password),
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            var moreInfo = new SingUpDto
            {
                PersonalIndefication = personalIndefication,
                Email = email,
            };

            var user = new User
            {
                LoginInfo = loginInfo,
                PersonalIndefication = moreInfo.PersonalIndefication,
                Email = moreInfo.Email
            };

            return user;
        }


        public byte[] HashPassword(string password)
        {
            string hashedPassword = _passwordHasher.HashPassword(null, password);
            return Encoding.UTF8.GetBytes(hashedPassword);
        }

        private bool VerifyPassword(LoginInfo loginInfo, string password)
        {
            string hashedPassword = Encoding.UTF8.GetString(loginInfo.Password);
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }




        private string GenerateJwtToken(string username, string role, long loginInfoId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginInfoId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var secretKey = _configuration.GetSection("Jwt:Key").Value;
            var issuer = _configuration.GetSection("Jwt:Issuer").Value;
            var audience = _configuration.GetSection("Jwt:Audience").Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> FindUserByNameAsync(string userName)
        {

            var existingUser = await _dbContext.Users
                .Include(u => u.LoginInfo)
                .FirstOrDefaultAsync(u => u.LoginInfo.UserName == userName);

            return existingUser;
        }

        public byte[] GeneratePasswordSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }

}
