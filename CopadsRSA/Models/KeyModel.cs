using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA.Models
{
    internal class KeyModel
    {
        public BigInteger Key { get; set; }
        public BigInteger Modulus { get; set; }

        public KeyModel(BigInteger key, BigInteger modulus)
        {
            Key = key;
            Modulus = modulus;
        }   
    }
}
