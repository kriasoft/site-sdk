// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserStore.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using App.Data;
    using Microsoft.AspNet.Identity;

    public class UserStore : IUserStore
    {
        private readonly DatabaseContext db;

        public UserStore(DatabaseContext db)
        {
            this.db = db;
        }

        public Task<bool> Create(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IUser> Find(string userId)
        {
            int id;

            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                throw new ArgumentException("The user's ID provided is not a valid integer.", "userId");
            }

            return await this.db.Users
                                .Where(u => u.UserID.Equals(id))
                                .Select(u => new IdentityUser { StoreUserId = u.UserID, UserName = u.UserName })
                                .SingleOrDefaultAsync();
        }

        public async Task<IUser> FindByUserName(string userName)
        {
            return await this.db.Users
                                .Where(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                                .Select(u => new IdentityUser { StoreUserId = u.UserID, UserName = u.UserName })
                                .SingleOrDefaultAsync();
        }
    }
}
