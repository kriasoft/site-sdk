// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomIdentityStoreContext.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System;
    using System.Threading.Tasks;

    using App.Data;
    using Microsoft.AspNet.Identity;

    public class CustomIdentityStoreContext : IIdentityStoreContext
    {
        private bool isDisposed;
        private DatabaseContext db;

        public CustomIdentityStoreContext(DatabaseContext db)
        {
            this.db = db;
            this.Logins = new UserLoginStore(this.db);
            this.Roles = new RoleStore(this.db);
            this.Secrets = new UserSecretStore(this.db);
            this.UserClaims = new UserClaimStore(this.db);
            this.Users = new UserStore(this.db);
        }

        public IUserLoginStore Logins { get; protected set; }

        public IRoleStore Roles { get; protected set; }

        public IUserSecretStore Secrets { get; protected set; }

        public IUserClaimStore UserClaims { get; protected set; }

        public IUserStore Users { get; protected set; }

        public virtual async Task SaveChanges()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("IdentityStoreManager");
            }

            var num = await this.db.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }

            this.isDisposed = true;
            this.db = (DatabaseContext)null;
            this.Users = (IUserStore)null;
            this.UserClaims = (IUserClaimStore)null;
            this.Roles = (IRoleStore)null;
            this.Secrets = (IUserSecretStore)null;
            this.Logins = (IUserLoginStore)null;
        }
    }
}
