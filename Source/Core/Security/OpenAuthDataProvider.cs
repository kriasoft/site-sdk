// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenAuthDataProvider.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Security
{
    using System;
    using System.Linq;

    using App.Data;

    using DotNetOpenAuth.AspNet;

    public class OpenAuthDataProvider : IOpenAuthDataProvider
    {
        private readonly DatabaseContext db;

        public OpenAuthDataProvider(DatabaseContext db)
        {
            this.db = db;
        }

        public string GetUserNameFromOpenAuth(string providerName, string providerUserID)
        {
            return this.db.UserOpenAuthAccounts
                          .Where(a => a.ProviderName == providerName && a.ProviderUserID == providerUserID)
                          .Select(a => a.User.UserName)
                          .SingleOrDefault();
        }
    }
}
