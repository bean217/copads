using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using CopadsRSA.Models;
using System.Reflection.Metadata;

namespace CopadsRSA
{
    internal class SecureMessaging
    {
        private HttpClient client;
        private static string apiUri = "http://kayrun.cs.rit.edu:5000";
        public SecureMessaging()
        {
            client = new HttpClient();
        }

        /// <summary>
        ///     Generates a public.key and private.key pair of keySize bits.
        ///     These keys are written to the current directory using base64 encoding.
        /// </summary>
        /// <param name="keySize">
        ///     Size of key pair in bits.
        /// </param>
        public void keyGen(int keySize)
        {
            if (keySize < 512 || keySize % 8 != 0)
            {
                throw new SMException("keySize must be at least 512 bits and divisible by 8");
            }
            // get an offset between 20% and 30% of the keySize
            var offset = RandomNumberGenerator.GetInt32(keySize / 5, (keySize * 3 / 10) + 1);
            // get a random bit to determine if offset is positive or negative
            var sign = RandomNumberGenerator.GetInt32(2) == 0 ? -1 : 1;

            ////Console.WriteLine($"offset: {(sign < 0 ? "-" : "")}{offset}");

            // determine number of p bits
            var pBits = (keySize / 2) + sign * offset;
            // assure pBits is divisible by 8
            pBits = pBits % 8 == 0 ? pBits : pBits - pBits % 8;
            // if pBits cannot be less than 256 bits or more than keySize - 256 bits
            pBits = pBits < 256 ? 256 : (pBits > keySize - 256 ? keySize - 256 : pBits);
            // determine number of q bits, the complement amount of pBits
            var qBits = keySize - pBits;

            ////Console.WriteLine($"# p bits: {pBits}");
            ////Console.WriteLine($"# q bits: {qBits}");

            // generate random prime BigInts for p and q
            var primeGen = new PrimeGen.PrimeGen();
            var p = primeGen.GeneratePrime(pBits);
            var q = primeGen.GeneratePrime(qBits);

            ////Console.WriteLine($"p: {p}");
            ////Console.WriteLine($"q: {q}");

            // calculate n and r based on p and q
            var N = p * q;
            var r = (p - 1) * (q - 1);
            // generate public key
            int E = 65537;
            var D = modInverse(E, r);

            // public key object
            var pubkey = new PublicKeyModel(encodeKey(E, N));

            // private key object
            var pvtkey = new PrivateKeyModel(encodeKey(D, N));

            // serialize public key and write it to public.key
            using (var outputFile = File.CreateText($"public.key"))
            {
                outputFile.Write(JsonConvert.SerializeObject(pubkey));
            }

            // seruakuze private key and write it to private.key
            using (var outputFile = File.CreateText($"private.key"))
            {
                outputFile.Write(JsonConvert.SerializeObject(pvtkey));
            }
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

            // deserialize the public and private key
            var pubkey = JsonConvert.DeserializeObject<PublicKeyModel>(File.ReadAllText("public.key"));
            var pvtkey = JsonConvert.DeserializeObject<PrivateKeyModel>(File.ReadAllText("private.key"));

            if (pvtkey == null || pubkey == null)
            {
                throw new SMException("Key Pair Does not exist");
            }
            // set/add email to the public/private keys
            pubkey.Email = email;
            // If the email has already been tracked in the private key, ignore it
            if (pvtkey.Email != null && !pvtkey.Email.Contains(email))
            {
                pvtkey.AddEmail(email);
            }

            var content = new StringContent(JsonConvert.SerializeObject(pubkey), Encoding.UTF8, "application/json");
            // PUT public key for email
            using HttpResponseMessage response = Task.Run(async () => await client.PutAsync($"{apiUri}/Key/{email}", content)).Result;
            // Throw exception on server error
            if (!response.IsSuccessStatusCode)
            {
                throw new SMException($"Failed to upload public key for: {email}");
            }

            // save private key data locally after 200 OK from the server
            using (var outputFile = File.CreateText($"private.key"))
            {
                outputFile.Write(JsonConvert.SerializeObject(pvtkey));
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
            try
            {
                // GET public key for email
                using HttpResponseMessage response = Task.Run(async () => await client.GetAsync($"{apiUri}/Key/{email}")).Result;
                // Throw exception on server error
                if (!response.IsSuccessStatusCode)
                {
                    throw new SMException($"Failed to retrieve public key for: {email}");
                }
                // Get the response body
                var responseBody = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
                // If the response body is empty, then no key exists for email
                if (responseBody.Length == 0)
                {
                    throw new SMException($"No public key found for: {email}");
                }
                
                // Write response to <email>.key
                using (var outputFile = File.CreateText($"{email}.key"))
                {
                    outputFile.Write(responseBody);
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Exception Thrown in getKey()");
            }
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
            // Ensure public key for email exists
            if (!File.Exists($"{email}.key"))
            {
                throw new SMException($"No public key found for: {email}");
            }
            // take the plaintext message and convert it to a byte array
            byte[] message = Encoding.UTF8.GetBytes(plaintext);
            // take the message byte array and load it into a big integer
            BigInteger messageData = new BigInteger(message, isUnsigned: true);

            // get the deserialize the public key for email
            var emailPubkey = JsonConvert.DeserializeObject<PublicKeyModel>(File.ReadAllText($"{email}.key"));

            if (emailPubkey == null || emailPubkey.Key == null)
            {
                throw new SMException($"Missing public key for: {email}");
            }

            KeyModel pubkey = decodeKey(emailPubkey.Key);

            // perform the encryption algorithm
            BigInteger cipherData = BigInteger.ModPow(messageData, pubkey.Key, pubkey.Modulus);
            // convert the resulting cipherData to a byte array
            byte[] cipherMessage = cipherData.ToByteArray();
            // Base64 encode the byte array
            string cipherMsgBase64 = Convert.ToBase64String(cipherMessage);
            // load the base64 encoded message into a message object
            var preparedMessage = new MessageModel(email, cipherMsgBase64);
            // send the message to the server
            var content = new StringContent(JsonConvert.SerializeObject(preparedMessage), Encoding.UTF8, "application/json");

            // PUT message for email
            using HttpResponseMessage response = Task.Run(async () => await client.PutAsync($"{apiUri}/Message/{email}", content)).Result;
            // Throw exception on server error
            if (!response.IsSuccessStatusCode)
            {
                throw new SMException($"Failed to upload message for: {email}");
            }
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
            // validate private key exists
            if (!File.Exists($"private.key"))
            {
                throw new SMException($"Private key does not exist");
            }
            // load json object from the server into a local object
            var pvtkey = JsonConvert.DeserializeObject<PrivateKeyModel>(File.ReadAllText($"private.key"));
            if (pvtkey == null || pvtkey.Key == null || pvtkey.Email == null || !pvtkey.Email.Contains(email))
            {
                throw new SMException($"Private key does not exist");
            }

            // GET message for email
            using HttpResponseMessage response = Task.Run(async () => await client.GetAsync($"{apiUri}/Message/{email}")).Result;
            // Throw exception on server error
            if (!response.IsSuccessStatusCode)
            {
                throw new SMException($"Failed to upload message for: {email}");
            }
            // Get the response body
            var responseBody = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
            // If the response body is empty, then no key exists for email
            if (responseBody.Length == 0)
            {
                throw new SMException($"No public key found for: {email}");
            }
            var messageObj = JsonConvert.DeserializeObject<MessageModel>(responseBody);
            if (messageObj == null || messageObj.Content == null)
            {
                throw new SMException($"No message from: {email}");
            }

            // Base64 decode the content property of the message into a byte array
            byte[] messageContent = Convert.FromBase64String(messageObj.Content);

            BigInteger messageData = new BigInteger(messageContent, isUnsigned: true);

            // perform the decryption algorithm
            var pvtkeyObj = decodeKey(pvtkey.Key);
            BigInteger plaintextData = BigInteger.ModPow(messageData, pvtkeyObj.Key, pvtkeyObj.Modulus);
            // convert plaintextData to a byte array, then to a string
            string plaintext = Encoding.UTF8.GetString(plaintextData.ToByteArray());
            return plaintext;
        }

        private static KeyModel decodeKey(string base64key)
        {
            byte[] bytes = Convert.FromBase64String(base64key);

            int e = BitConverter.ToInt32(bytes.Take(4).Reverse().ToArray());

            BigInteger E = new BigInteger(bytes.Skip(4).Take(e).ToArray(), isUnsigned: true);

            int n = BitConverter.ToInt32(bytes.Skip(4).Skip(e).Take(4).Reverse().ToArray());

            BigInteger N = new BigInteger(bytes.Skip(4).Skip(e).Skip(4).Take(n).ToArray(), isUnsigned: true);

            return new KeyModel(E, N);
        }

        private static string encodeKey(BigInteger E, BigInteger N)
        {
            byte[] eBytes = BitConverter.GetBytes(E.GetByteCount()).Reverse().ToArray();
            byte[] EBytes = E.ToByteArray();
            byte[] nBytes = BitConverter.GetBytes(N.GetByteCount()).Reverse().ToArray();
            byte[] NBytes = N.ToByteArray();

            byte[] bytes = eBytes.Concat(EBytes).Concat(nBytes).Concat(NBytes).ToArray();

            return Convert.ToBase64String(bytes);
        }



        private static BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0)
            {
                v = (v + n) % n;
            }
            return v;
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
