// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserLoginStore.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using App.Data;
    using Microsoft.AspNet.Identity;

    public class UserLoginStore : IUserLoginStore
    {
        private readonly DatabaseContext db;

        public UserLoginStore(DatabaseContext db)
        {
            this.db = db;
        }

        public Task<bool> Add(IUserLogin login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<IUserLogin>> GetLogins(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetProviderKey(string userId, string loginProvider)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserId(string loginProvider, string providerKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(string userId, string loginProvider, string providerKey)
        {
            throw new NotImplementedException();
        }
    }
}
