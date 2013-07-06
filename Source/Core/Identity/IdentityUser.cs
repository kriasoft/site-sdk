// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityUser.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System.Globalization;

    using Microsoft.AspNet.Identity;

    public class IdentityUser : IUser
    {
        public string Id
        {
            get { return this.StoreUserId.ToString(CultureInfo.InvariantCulture); }
            set { this.StoreUserId = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture); }
        }

        public int StoreUserId { get; set; }

        public string UserName { get; set; }
    }
}
