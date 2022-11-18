/// Author: Benjamin Piro
/// Email: brp8396@rit.edu
/// File Description: Represents a public key that can be stored locally.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    /// <summary>
    ///     Represents a public key that can be stored locally
    /// </summary>
    internal class PublicKeyModel
    {
        /// <summary>
        ///     Email that the public key is valid for
        ///     This will always be locally stored as NULL
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        ///     Base64 encoded key value
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        ///     Public key constructor
        /// </summary>
        /// <param name="key">Encoded key value</param>
        public PublicKeyModel(string? key)
        {
            this.Key = key;
        }
    }
}
