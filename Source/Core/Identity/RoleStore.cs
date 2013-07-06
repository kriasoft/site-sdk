// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleStore.cs" company="KriaSoft LLC">
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

    public class RoleStore : IRoleStore
    {
        private readonly DatabaseContext db;

        public RoleStore(DatabaseContext db)
        {
            this.db = db;
        }

        public Task<bool> AddUserToRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateRole(IRole role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRole(string roleId, bool failIfNonEmpty)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRolesForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUsersInRoles(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInRole(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RoleExists(string roleId)
        {
            throw new NotImplementedException();
        }
    }
}
