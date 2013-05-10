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
    using System.Web;
    using System.Web.Mvc;

    using App.Data;
    using App.Properties;
    using App.Security;

    using DotNetOpenAuth.AspNet;
    using Microsoft.AspNet.Membership.OpenAuth;

    public interface IMembershipService
    {
        void RegisterModelState(ModelStateDictionary modelState);

        User CreateUser(string userName, string email, string password, string providerName = null, string providerUserID = null, string providerUserName = null);

        bool ValidateUser(string userName, string password, bool updateLastLoginDate = true);

        bool ValidateOpenAuthUser(string providerName, string providerUserID, bool createPersistentCookie);

        /// <summary>Adds an external login account to an existing membership user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <param name="providerName">The name of the external authentication provider.</param>
        /// <param name="providerUserID">The user ID of the user with the external authentication provider.</param>
        /// <param name="providerUserName">The user name of the user with the external authentication provider.</param>
        void AddOpenAuthAccount(string userName, string providerName, string providerUserID, string providerUserName);
    }

    public class MembershipService : IMembershipService
    {
        private readonly DatabaseContext db;
        
        private System.Web.Mvc.ModelStateDictionary modelState;

        public MembershipService(DatabaseContext db)
        {
            this.db = db;
        }

        private bool IsModelStateValid
        {
            get { return this.modelState == null || this.modelState.IsValid; }
        }

        public void RegisterModelState(ModelStateDictionary modelState)
        {
            this.modelState = modelState;
        }

        public User CreateUser(string userName, string email, string password, string providerName = null, string providerUserID = null, string providerUserName = null)
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

            if (!string.IsNullOrWhiteSpace(providerName))
            {
                user.OpenAuthAccounts.Add(new UserOpenAuthAccount { ProviderName = providerName, ProviderUserID = providerUserID, ProviderUserName = providerUserName, LastUsedDate = dateNow });
            }

            try
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();
            }
            catch (DataException ex)
            {
                // Username already exists
                if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("UK_User_UserName"))
                {
                    this.AddModelError(string.Format("The username '{0}' is already registered.", userName));
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return user;
        }

        public bool ValidateUser(string userName, string password, bool updateLastLoginDate = true)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length > 128 ||
                string.IsNullOrWhiteSpace(password) || password.Length > 256)
            {
                this.AddModelError("The user name or password provided is incorrect.");
                return false;
            }

            var user = this.db.Users.SingleOrDefault(u => u.UserName == userName);

            if (user == null || !PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
            {
                this.AddModelError("The user name or password provided is incorrect.");
                return false;
            }

            if (!user.IsApproved)
            {
                this.AddModelError("Login failed. Your account is not approved.");
                return false;
            }

            if (updateLastLoginDate)
            {
                var dateNow = DateTime.UtcNow;
                user.LastLoginDate = dateNow;
                user.LastActivityDate = dateNow;
            }

            this.db.SaveChanges();
            return true;
        }

        public bool ValidateOpenAuthUser(string providerName, string providerUserID, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "providerName");
            }

            if (string.IsNullOrEmpty(providerUserID))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "providerUserID");
            }

            var openAuthAccount = this.db.UserOpenAuthAccounts.SingleOrDefault(a => a.ProviderName == providerName && a.ProviderUserID == providerUserID);

            if (openAuthAccount == null)
            {
                return false;
            }

            openAuthAccount.LastUsedDate = DateTime.UtcNow;
            this.db.SaveChanges();

            var manager = new OpenAuthSecurityManager(new HttpContextWrapper(HttpContext.Current), OpenAuth.AuthenticationClients.GetByProviderName(providerName), new OpenAuthDataProvider(this.db));
            return manager.Login(providerUserID, createPersistentCookie);
        }

        public void AddOpenAuthAccount(string userName, string providerName, string providerUserID, string providerUserName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "userName");
            }

            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "providerName");
            }

            if (string.IsNullOrEmpty(providerUserID))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "providerUserID");
            }

            if (string.IsNullOrEmpty(providerUserName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "providerUserName");
            }

            var user = this.db.Users.SingleOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                throw new InvalidOperationException(string.Format(Resources.MembershipUserNotFound, userName));
            }

            var dateNow = DateTime.UtcNow;

            this.db.UserOpenAuthAccounts.Add(new UserOpenAuthAccount
            {
                UserID = user.UserID,
                ProviderName = providerName,
                ProviderUserID = providerUserID,
                ProviderUserName = providerUserName,
                LastUsedDate = dateNow
            });

            user.LastLoginDate = dateNow;
            user.LastActivityDate = dateNow;
            this.db.SaveChanges();
        }

        private void AddModelError(string errorMessage)
        {
            this.AddModelError(string.Empty, errorMessage);
        }

        private void AddModelError(string key, string errorMessage)
        {
            if (this.modelState == null)
            {
                throw new ApplicationException(errorMessage);
            }

            this.modelState.AddModelError(key, errorMessage);
        }
    }
}
