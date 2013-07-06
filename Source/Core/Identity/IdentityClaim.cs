// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityClaim.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System.Globalization;

    using Microsoft.AspNet.Identity;

    public class IdentityClaim : IUserClaim
    {
        public string UserId
        {
            get { return this.StoreUserId.ToString(CultureInfo.InvariantCulture); }
            set { this.StoreUserId = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture); }
        }

        public int StoreUserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
