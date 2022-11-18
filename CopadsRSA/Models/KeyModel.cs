/// Author: Benjamin Piro
/// Email: brp8396@rit.edu
/// File Description: Model class for representing a logical RSA (public or private) key.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA.Models
{
    /// <summary>
    ///     Model class for representing a logical RSA (public or private) key.
    /// </summary>
    internal class KeyModel
    {
        /// <summary>
        ///     RSA public/private key value
        /// </summary>
        public BigInteger Key { get; set; }
        /// <summary>
        ///     RSA modulus ('N') value
        /// </summary>
        public BigInteger Modulus { get; set; }
        
        /// <summary>
        ///     KeyModel constructor
        /// </summary>
        /// <param name="key">RSA key</param>
        /// <param name="modulus">RSA Modulus</param>
        public KeyModel(BigInteger key, BigInteger modulus)
        {
            Key = key;
            Modulus = modulus;
        }   
    }
}
