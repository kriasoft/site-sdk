// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMembershipService.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Http.ModelBinding;

    using App.Data;
    using App.Properties;
    using App.Security;

    using DotNetOpenAuth.AspNet;
    using Microsoft.AspNet.Membership.OpenAuth;

    public interface IMembershipService
    {
        void RegisterModelState(ModelStateDictionary modelState);

        User CreateUser(string userName, string email, string password, string displayName, string providerName = null, string providerUserID = null, string providerUserName = null);

        bool ValidateUser(string userName, string password, bool updateLastLoginDate = true);

        bool ValidateOpenAuthUser(string providerName, string providerUserID, bool createPersistentCookie);

        /// <summary>Adds an external login account to an existing membership user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <param name="providerName">The name of the external authentication provider.</param>
        /// <param name="providerUserID">The user ID of the user with the external authentication provider.</param>
        /// <param name="providerUserName">The user name of the user with the external authentication provider.</param>
        void AddOpenAuthAccount(string userName, string providerName, string providerUserID, string providerUserName);

        /// <summary>Removes an external login account from an existing membership user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <param name="providerName">The name of the external authentication provider.</param>
        /// <param name="providerUserID">The user ID of the user with the external authentication provider.</param>
        /// <param name="providerUserName">The user name of the user with the external authentication provider.</param>
        /// <returns>True if account was deleted; False otherwise.</returns>
        bool RemoveOpenAuthAccount(string userName, string providerName, string providerUserID);

        /// <summary>Gets a list of external login accounts of an existing membership user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <returns>A list of external login accounts.</returns>
        ICollection<UserOpenAuthAccount> GetOpenAuthAccounts(string userName);

        /// <summary>Determins if user has a previously set password.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <returns>True if user has a password; False otherwise.</returns>
        bool HasPassword(string userName);

        /// <summary>Changes the password for the specified user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <param name="currentPassword">The current password for the user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if operation succeded; False otherwise.</returns>
        bool ChangePassword(string userName, string currentPassword, string newPassword);

        /// <summary>Updates the password for the specified user.</summary>
        /// <param name="userName">The user name of the local membership user.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if password was successfully set; False otherwise.</returns>
        void SetPassword(string userName, string password);
    }

    public class MembershipService : IMembershipService
    {
        private readonly DatabaseContext db;
        
        private ModelStateDictionary modelState;

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

        public User CreateUser(string userName, string email, string password, string displayName, string providerName = null, string providerUserID = null, string providerUserName = null)
        {
            var dateNow = DateTime.UtcNow;
            var hash = string.IsNullOrEmpty(password) ? PasswordHash.Empty : PasswordHash.Create(password);

            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = hash.Hash,
                PasswordSalt = hash.Salt,
                DisplayName = displayName,
                IsApproved = true,
                CreatedDate = dateNow,
                LastLoginDate = dateNow,
                LastActivityDate = dateNow
            };

            if (db.Users.Where(u => u.UserName == userName).Any())
            {
                this.AddModelError(string.Format("The username '{0}' is already registered.", userName));
                return null;
            }

            if (db.Users.Any(u => u.Email == email))
            {
                this.AddModelError(string.Format("A user with email '{0}' is already registered."), email);
                return null;
            }

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

            if (user == null || user.PasswordHash.Length == 0 || !PasswordHash.Validate(password, user.PasswordHash, user.PasswordSalt))
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

        public bool RemoveOpenAuthAccount(string userName, string providerName, string providerUserID)
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

            var openAuthAccount = this.db.UserOpenAuthAccounts.SingleOrDefault(a => a.ProviderName == providerName && a.ProviderUserID == providerUserID && a.User.UserName == userName);

            if (openAuthAccount == null)
            {
                return false;
            }

            this.db.UserOpenAuthAccounts.Remove(openAuthAccount);
            this.db.SaveChanges();
            return true;
        }

        public ICollection<UserOpenAuthAccount> GetOpenAuthAccounts(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "userName");
            }

            return this.db.UserOpenAuthAccounts.Where(a => a.User.UserName == userName).ToList().AsReadOnly();
        }

        public bool HasPassword(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "userName");
            }

            var user = this.db.Users.Where(u => u.UserName == userName).Select(u => new { UserName = u.UserName, PasswordHash = u.PasswordHash }).SingleOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException(string.Format(Resources.MembershipUserNotFound, userName));
            }

            return user.PasswordHash.Length > 0;
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "userName");
            }

            var user = this.db.Users.Where(u => u.UserName == userName).SingleOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException(string.Format(Resources.MembershipUserNotFound, userName));
            }

            if (!PasswordHash.Validate(currentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return false;
            }

            var dateNow = DateTime.UtcNow;
            var hash = PasswordHash.Create(newPassword);
            user.PasswordHash = hash.Hash;
            user.PasswordSalt = hash.Salt;
            user.LastActivityDate = dateNow;
            user.LastPasswordChangedDate = dateNow;
            db.SaveChanges();
            return true;
        }

        public void SetPassword(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(Resources.ArgumentNullOrEmpty, "userName");
            }

            var user = this.db.Users.Where(u => u.UserName == userName).SingleOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException(string.Format(Resources.MembershipUserNotFound, userName));
            }

            var dateNow = DateTime.UtcNow;
            var hash = PasswordHash.Create(password);
            user.PasswordHash = hash.Hash;
            user.PasswordSalt = hash.Salt;
            user.LastActivityDate = dateNow;
            user.LastPasswordChangedDate = dateNow;
            db.SaveChanges();
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
