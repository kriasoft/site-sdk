// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSecretStore.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System;
    using System.Threading.Tasks;

    using App.Data;
    using Microsoft.AspNet.Identity;

    public class UserSecretStore : IUserSecretStore
    {
        private readonly DatabaseContext db;

        public UserSecretStore(DatabaseContext db)
        {
            this.db = db;
        }

        public Task<bool> Create(IUserSecret userSecret)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<IUserSecret> Find(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(string userName, string newSecret)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Validate(string userName, string loginSecret)
        {
            throw new NotImplementedException();
        }
    }
}
