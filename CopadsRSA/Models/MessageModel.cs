using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    internal class MessageModel
    {
        public string? Email { get; set; }
        public string? Content { get; set; }
    
        public MessageModel(string? email, string? content)
        {
            this.Email = email;
            this.Content = content;
        }
    }
}
