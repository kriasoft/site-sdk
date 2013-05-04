// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMembershipService.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Services
{
    using System;
    using System.Data;
    using System.Linq;

    using App.Data;
    using App.Security;

    public interface IMembershipService
    {
        User CreateUser(string userName, string email, string password);

        void ValidateUser(string userName, string password);
    }

    public class MembershipService : IMembershipService
    {
        private DatabaseContext db;

        public MembershipService(DatabaseContext db)
        {
            this.db = db;
        }

        public User CreateUser(string userName, string email, string password)
        {
            var dateNow = DateTime.UtcNow;
            var hash = PasswordHash.Create(password);

            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = hash.Hash,
                PasswordSalt = hash.Salt,
                IsApproved = true,
                CreatedDate = dateNow,
                LastLoginDate = dateNow,
                LastActivityDate = dateNow
            };

            try
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();
            }
            catch (DataException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("UK_User_UserName"))
                {
                    throw new ApplicationException(string.Format("The username '{0}' is already registered.", userName));
                }
                else
                {
                    throw ex;
                }
            }

            return user;
        }

        public void ValidateUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length > 128 ||
                string.IsNullOrWhiteSpace(password) || password.Length > 256)
            {
                throw new ApplicationException("The user name or password provided is incorrect.");
            }

            var user = this.db.Users.SingleOrDefault(u => u.UserName == userName);

            if (user == null || !PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new ApplicationException("The user name or password provided is incorrect.");
            }

            if (!user.IsApproved)
            {
                throw new ApplicationException("Login failed. Your account is not approved.");
            }

            var dateNow = DateTime.UtcNow;
            user.LastLoginDate = dateNow;
            user.LastActivityDate = dateNow;
            this.db.SaveChanges();
        }
    }
}
