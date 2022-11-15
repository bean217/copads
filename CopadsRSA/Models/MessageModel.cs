using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    /// <summary>
    ///     Represents a logical message for a particular user
    /// </summary>
    internal class MessageModel
    {
        /// <summary>
        ///     Email of the recipient
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        ///     Message (usually encrypted)
        /// </summary>
        public string? Content { get; set; }
    
        /// <summary>
        ///     Message constructor
        /// </summary>
        /// <param name="email">Recipient's email</param>
        /// <param name="content">Message</param>
        public MessageModel(string? email, string? content)
        {
            this.Email = email;
            this.Content = content;
        }
    }
}
