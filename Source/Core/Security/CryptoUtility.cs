// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoUtility.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Security
{
    using System;
    using System.Text;
    using System.Web.Security;

    /// <summary>An utility class for cryptographic related tasks.</summary>
    public static class CryptoUtility
    {
        private const char Delimiter = '\u25AC';

        /// <summary>Encrypts provided text strings and produces serialized Base64 output.</summary>
        /// <param name="purpose">A purpose for the data.</param>
        /// <param name="values">A list of values.</param>
        /// <returns>A base64 encoded and protected string.</returns>
        public static string Serialize(string purpose, params string[] values)
        {
            var data = Encoding.Unicode.GetBytes(string.Join(Delimiter.ToString(), values));
            return Convert.ToBase64String(MachineKey.Protect(data, purpose));
        }

        /// <summary>Deserializes base64 encoded and protected string back into en-encrypted string values.</summary>
        /// <param name="purpose">A purpose for the data.</param>
        /// <param name="serializedData">A base64 encoded and protected string.</param>
        /// <returns>An array of decoded string values.</returns>
        public static string[] Deserialize(string purpose, string serializedData)
        {
            var data = MachineKey.Unprotect(Convert.FromBase64String(serializedData), purpose);
            return Encoding.Unicode.GetString(data).Split(Delimiter);
        }

        public static string[] TryDeserialize(string purpose, string serializedData)
        {
            try
            {
                return Deserialize(purpose, serializedData);
            }
            catch
            {
                return null;
            }
        }
    }
}
