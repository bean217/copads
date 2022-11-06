using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    internal class PrivateKeyModel
    {
        public IEnumerable<string>? Email { get; set; }
        public string? Key { get; set; }
    }
}
