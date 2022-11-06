using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopadsRSA
{
    internal class SecureMessaging
    {
        /// <summary>
        ///     Generates a public.key and private.key pair of keySize bits.
        ///     These keys are written to the current directory using base64 encoding.
        /// </summary>
        /// <param name="keySize">
        ///     Size of key pair in bits.
        /// </param>
        public void keyGen(int keySize)
        {

        }

        /// <summary>
        ///     Sends public.key to the server with the given email address.
        ///     Also adds the email to the private key.
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="SMException">
        ///     Thrown if public.key and private.key have not yet been generated
        /// </exception>
        public void sendKey(string email)
        {
            if (!File.Exists("public.key") || !File.Exists("private.key"))
            {
                throw new SMException("Key Pair Does not exist");
            }
        }

        /// <summary>
        ///     Retrieves the public key for a particular user, stored locally as <email>.key.
        /// </summary>
        /// <param name="email">
        ///     Email of user whose public key is being fetched
        /// </param>
        public void getKey(string email)
        {

        }

        /// <summary>
        ///     Encrypts and encodes a plaintext message, then sends it to the server for
        ///     a recipient to receive.
        /// </summary>
        /// <param name="email">
        ///     Email of the user being sent a message
        /// </param>
        /// <param name="plaintext">
        ///     Plaintext message being sent to a user
        /// </param>
        public void sendMsg(string email, string plaintext)
        {

        }

        /// <summary>
        ///     Retrieves and decodes a message from a user.
        /// </summary>
        /// <param name="email">
        ///     Email of sender/user
        /// </param>
        /// <returns>
        ///     Decoded plaintext message from a user
        /// </returns>
        public string getMsg(string email)
        {
            return "";
        }
    }

    /// <summary>
    ///     Exception generated from Secure Messaging
    /// </summary>
    internal class SMException : Exception {
        /// <summary>
        ///     Constructs a Secure Messaging Exception without any properties.
        /// </summary>
        public SMException() : base() { }
        /// <summary>
        ///     Constructs a Secure Messaging Exception with a message property
        /// </summary>
        /// <param name="message">
        ///     SMException message
        /// </param>
        public SMException(string message) : base(message) { }
    }
}
