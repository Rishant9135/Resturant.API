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

namespace ElectionData.Common.Facade
{
    public class UserFacade : Facade<UserTable, UserModel>
    {
        private readonly IReadOnlyRepository<UserTable> userRepository;
        public UserFacade(IReadOnlyRepository<UserTable> userRepository) : base(userRepository)
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
    }
}
