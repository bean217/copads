using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    internal class PrivateKeyModel
    {
        public List<string>? Email { get; set; }
        public string? Key { get; set; }

        /// <summary>
        ///     If List<string> Email is null, allocate a new list and email.
        /// </summary>
        /// <param name="email">Email being added to private key</param>
        public void AddEmail(string email)
        {
            Email ??= new List<string>();
            Email.Add(email);
        }

        public PrivateKeyModel(string? key)
        {
            this.Email = new List<string>();
            this.Key = key;
        }
    }
}
