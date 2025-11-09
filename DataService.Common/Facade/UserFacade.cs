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
        private readonly IRepository<PasswordTbl> passwordRepository;
        private readonly IRepository<UserSessionTbl> userSessionRepository;
        public UserFacade(IRepository<UserTable> userRepository,
                            IRepository<PasswordTbl> passwordRepository,
                            IRepository<UserSessionTbl> userSessionRepository) : base(userRepository)
        {
            this.userRepository = userRepository;
            this.passwordRepository = passwordRepository;
            this.userSessionRepository = userSessionRepository;
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
            var user = userRepository
                .ListAll()
                .FirstOrDefault(u => u.Phone == username || u.Email == username);

            if (user == null)
                return null;

            var passwordHasher = new PasswordHasher<UserTable>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return verificationResult == PasswordVerificationResult.Success ? user : null;
        }
        public void SaveToken(string token, long userId)
        {
            var existingSession = userSessionRepository
                .ListAll()
                .FirstOrDefault(s => s.UserId == userId);

            var issuedAtIST = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            );

            if (existingSession != null)
            {
                existingSession.JwtToken = token;
                existingSession.IssuedAt = issuedAtIST;
                existingSession.ExpiresAt = issuedAtIST.AddDays(30); // Example: 30-Days expiry
                existingSession.UpdatedOn = issuedAtIST;
                existingSession.IsActive = true;

                userSessionRepository.Update(existingSession);
            }
            else
            {
                // Insert new session
                var userSession = new UserSessionTbl
                {
                    UserId = userId,
                    Username = "Default User Name",
                    JwtToken = token,
                    IssuedAt = issuedAtIST,
                    CreatedOn = issuedAtIST,
                    ExpiresAt = issuedAtIST.AddDays(30), // Example: 30-Days expiry
                    IsActive = true
                };

                userSessionRepository.Insert(userSession);
            }
        }
        public (bool IsSuccess, string Message) RegisterUser(UserRegisterModel register)
        {
            var existenceMessage = CheckUserExistence(register.Phone, register.Email);
            var user = new UserTable();
            if (!string.IsNullOrEmpty(existenceMessage))
            {
                return (false, existenceMessage);
            }

            var passwordHasher = new PasswordHasher<UserTable>();
            user.PasswordHash = passwordHasher.HashPassword(user, register.Password);
            user.Email = register.Email;
            user.FullName = register.FullName;
            user.FullAddress = register.FullAddress;
            user.Phone = register.Phone;
            user.IsActive = true;
            user.CreatedOn = DateTime.UtcNow;

            userRepository.Insert(user);

            SavePlainPassword((long)user.Id, register.Password);

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
        private void SavePlainPassword(long userId, string plainPassword)
        {
            var credential = new PasswordTbl
            {
                UserId = userId,
                Password = plainPassword,
                CreatedOn = DateTime.UtcNow
            };
            passwordRepository.Insert(credential);
        }
        public UserModel? GetUserProfile(long userId)
        {
            var user = userRepository.Get(userId);
            if (user == null)
                return null;

            return new UserModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                FullAddress = user.FullAddress,
                CreatedOn = user.CreatedOn,
                IsActive = user.IsActive
            };
        }
    }
}
