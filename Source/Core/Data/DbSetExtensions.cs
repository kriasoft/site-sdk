// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSetExtensions.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using App.Security;

    public static class DbSetExtensions
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

        public static async Task<UserNameUniquenessResult> CheckUniqueness(this DbSet<User> users, string userName, string email)
        {
            var result = await users.Where(u => u.UserName == userName || u.Email == email)
                                    .Select(u => new { UserName = u.UserName, Email = u.Email }).ToListAsync();

            return new UserNameUniquenessResult
            {
                IsUserNameUnique = result.Any(u => u.UserName == userName),
                IsEmailUnique = result.Any(u => u.Email == email)
            };
        }

        public static async Task<User> GetByUserNameOrEmailAndPassword(this DbSet<User> users, string userNameOrEmail, string password)
        {
            if (userNameOrEmail.Contains('@'))
            {
                foreach (var user in await users.Where(u => u.Email == userNameOrEmail).ToListAsync())
                {
                    if (PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
                    {
                        return user;
                    }
                }
            }
            else
            {
                var user = await users.SingleOrDefaultAsync(u => u.UserName == userNameOrEmail);

                if (user != null && PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
                {
                    return user;
                }
            }

            return null;
        }

        public static async Task<List<UserRole>> GetUserRoles(this DbSet<User> users, int userID)
        {
            return await users.Where(u => u.UserID.Equals(userID)).SelectMany(u => u.Roles).ToListAsync();
        }

        public struct UserNameUniquenessResult
        {
            public bool IsUserNameUnique;
            public bool IsEmailUnique;
        }
    }
}
