using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    internal class PublicKeyModel
    {
        public string? Email { get; set; }
        public string? Key { get; set; }

        public PublicKeyModel(string? key)
        {
            this.Key = key;
        }
    }
}
