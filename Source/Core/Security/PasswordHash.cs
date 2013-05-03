// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordHash.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Security
{
    using System.Security.Cryptography;

    /// <summary>
    /// An utility class for salted password hashing with PBKDF2-SHA1
    /// </summary>
    internal class PasswordHash
    {
        public const int SaltBytes = 24;
        public const int HashBytes = 24;
        public const int HashIterations = 1000;

        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int HashIndex = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHash"/> class.
        /// </summary>
        /// <param name="hash">Password hash</param>
        /// <param name="salt">Password salt</param>
        private PasswordHash(byte[] hash, byte[] salt)
        {
            this.Hash = hash;
            this.Salt = salt;
        }

        /// <summary>
        /// Gets password hash
        /// </summary>
        public byte[] Hash { get; private set; }

        /// <summary>
        /// Gets password salt
        /// </summary>
        public byte[] Salt { get; private set; }

        /// <summary>
        /// Creates a salted PBKDF2 hash of the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash and salt of the password.</returns>
        public static PasswordHash Create(string password)
        {
            // Generate a random salt
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltBytes];
            csprng.GetBytes(salt);

            // Hash the password and encode the parameters
            var hash = PBKDF2(password, salt, HashIterations, HashBytes);
            return new PasswordHash(hash, salt);
        }

        /// <summary>
        /// Validates a password against given hash and salt.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="hash">A hash of the correct password.</param>
        /// <param name="salt">A salt of the correct password.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public static bool Validate(string password, byte[] hash, byte[] salt)
        {
            var testHash = PBKDF2(password, salt, HashIterations, hash.Length);
            return Equals(hash, testHash);
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison method is used so that password hashes
        /// cannot be extracted from on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        private static bool Equals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}
