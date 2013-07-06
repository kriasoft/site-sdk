// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimStore.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using App.Data;
    using Microsoft.AspNet.Identity;

    public class UserClaimStore : IUserClaimStore
    {
        private readonly DatabaseContext db;

        public UserClaimStore(DatabaseContext db)
        {
            this.db = db;
        }

        public Task<bool> Add(IUserClaim userClaim)
        {
            if (userClaim == null)
            {
                throw new ArgumentNullException("userClaim");
            }

            var newClaim = new UserClaim
            {
                ClaimID = Guid.NewGuid(),
                UserID = int.Parse(userClaim.UserId, NumberStyles.Integer, CultureInfo.InvariantCulture),
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };

            this.db.UserClaims.Add(newClaim);
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<IUserClaim>> GetUserClaims(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            int id;

            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                throw new ArgumentException("The user's ID provided is not a valid integer.", "userId");
            }

            return await this.db.UserClaims.Where(c => c.UserID.Equals(id))
                                .Select(c => new IdentityClaim
                                {
                                    StoreUserId = c.UserID,
                                    ClaimType = c.ClaimType,
                                    ClaimValue = c.ClaimValue
                                })
                                .ToListAsync();
        }

        public async Task<bool> Remove(string userId, string claimType, string claimValue)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            if (claimType == null)
            {
                throw new ArgumentNullException("claimType");
            }

            int id;

            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                throw new ArgumentException("The user's ID provided is not a valid integer.", "userId");
            }

            var userClaim = await this.db.UserClaims.SingleOrDefaultAsync(c => c.UserID.Equals(id) && c.ClaimType.Equals(claimType) && c.ClaimValue.Equals(claimValue));

            if (userClaim == null)
            {
                return false;
            }

            this.db.UserClaims.Remove(userClaim);
            return true;
        }
    }
}
