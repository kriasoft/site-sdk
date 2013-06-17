// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContextExtensions.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using App.Security;
    using App.Properties;

    public static class DatabaseContextExtensions
    {
        public static User Add(this DbSet<User> users, string userName, string email, string password, string displayName)
        {
            var now = DateTime.UtcNow;
            var pwd = PasswordHash.Create(password);

            return users.Add(new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = pwd.Hash,
                PasswordSalt = pwd.Salt,
                DisplayName = displayName,
                IsApproved = true,
                CreatedDate = now,
                LastLoginDate = now,
                LastActivityDate = now
            });
        }

        public static void CheckUniqueness(this DbSet<User> users, string userName, string email, out bool isUserNameUnique, out bool isEmailUnique)
        {
            var result = users.Where(u => u.UserName == userName || u.Email == email)
                              .Select(u => new { UserName = u.UserName, Email = u.Email }).ToList();
            
            isUserNameUnique = result.Any(u => u.UserName == userName);
            isEmailUnique = result.Any(u => u.Email == email);
        }

        public static User GetByUserNameOrEmail(this DbSet<User> users, string userNameOrEmail, string password)
        {
            if (userNameOrEmail.Contains('@'))
            {
                foreach (var user in users.Where(u => u.Email == userNameOrEmail))
                {
                    if (PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
                    {
                        return user;
                    }
                }
            }
            else
            {
                var user = users.SingleOrDefault(u => u.UserName == userNameOrEmail);

                if (user != null && PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
                {
                    return user;
                }
            }

            return null;
        }
    }
}
