using BSPL.Domain;
using ElectionData.Common.Models;
using System;
using System.Collections.Generic;
using BSPL.Facades;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Identity;

namespace ElectionData.Common.Facade
{
    public class UserFacade : Facade<UserTable, UserModel>
    {
        private readonly IRepository<UserTable> userRepository;
        public UserFacade(IRepository<UserTable> userRepository) : base(userRepository)
        {
            this.userRepository = userRepository;
        }
        public bool AuthenticateUser(string username, string password)
        {
            // You should hash the password before comparing in production
            var user = userRepository
                .ListAll()
                .FirstOrDefault(u => u.Phone == username && u.PasswordHash == password);

            return user != null;
        }
        public UserTable? AuthenticateUserJwt(string username, string password)
        {
            // In production, hash & compare passwords
            return userRepository
                .ListAll()
                .FirstOrDefault(u => u.Phone == username && u.PasswordHash == password);
        }

        public (bool IsSuccess, string Message) RegisterUser(UserTable user)
        {
            var existenceMessage = CheckUserExistence(user.Phone, user.Email);
            if (!string.IsNullOrEmpty(existenceMessage))
            {
                return (false, existenceMessage);
            }

            var passwordHasher = new PasswordHasher<UserTable>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);

            user.CreatedOn = DateTime.UtcNow;

            userRepository.Insert(user);

            return (true, "User registered successfully.");
        }
        private string CheckUserExistence(string? phone, string? email)
        {
            var users = userRepository.ListAll();

            if (!string.IsNullOrEmpty(phone) &&
                users.Any(u => u.Phone != null && u.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase)))
            {
                return $"User already exists with this phone number: {phone}";
            }

            if (!string.IsNullOrEmpty(email) &&
                users.Any(u => u.Email != null && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return $"User already exists with this email: {email}";
            }

            return string.Empty; // no duplicate found
        }
    }
}
