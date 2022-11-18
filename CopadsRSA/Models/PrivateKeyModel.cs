/// Author: Benjamin Piro
/// Email: brp8396@rit.edu
/// File Description: Represents a private key that can be stored locally.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    /// <summary>
    ///     Represents a private key that can be stored locally
    /// </summary>
    internal class PrivateKeyModel
    {
        /// <summary>
        ///     List of emails that the private key is valid for
        /// </summary>
        public List<string>? Email { get; set; }
        /// <summary>
        ///     Base64 encoded key value
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        ///     If List<string> Email is null, allocate a new list and email
        /// </summary>
        /// <param name="email">Email being added to private key</param>
        public void AddEmail(string email)
        {
            Email ??= new List<string>();
            Email.Add(email);
        }

        /// <summary>
        ///     Private key constructor
        /// </summary>
        /// <param name="key">Encoded key value</param>
        public PrivateKeyModel(string? key)
        {
            this.Email = new List<string>();
            this.Key = key;
        }
    }
}
