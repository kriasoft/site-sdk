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
    using System.Web.Mvc;

    using App.Data;
    using App.Security;

    public interface IMembershipService
    {
        void RegisterModelState(ModelStateDictionary modelState);

        User CreateUser(string userName, string email, string password);

        bool ValidateUser(string userName, string password, bool updateLastLoginDate = true);
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
                // Username already exists
                if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("UK_User_UserName"))
                {
                    this.AddModelError(string.Format("The username '{0}' is already registered.", userName));
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
