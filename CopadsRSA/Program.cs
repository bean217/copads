/// Author: Benjamin Piro
/// Email: brp8396@rit.edu
/// File Description: Startup program for running CopadsRSA command line interface.

using CopadsRSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace copadsRSA
{
    /// <summary>
    ///     Main class for running CopadsRSA command line interface
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Main entry point to CopadsRSA command line interface
        /// </summary>
        /// <param name="args">Program arguments</param>
        public static void Main(string[] args)
        {
            if (args.Length >= 2 && args.Length <= 3)
            {
                var secureMsgClient = new SecureMessaging();
                var command = args[0].ToLower();

                try
                {
                    switch (command)
                    {
                        case "keygen":
                            if (args.Length != 2)
                            {
                                throw new SMException("Too many arguments");
                            }
                            int numBits;
                            if (!int.TryParse(args[1], out numBits))
                            {
                                throw new SMException("Invalid Keysize");
                            }
                            secureMsgClient.KeyGen(numBits);
                            break;
                        case "sendkey":
                            if (args.Length != 2)
                            {
                                throw new SMException("Too many arguments");
                            }
                            secureMsgClient.SendKey(args[1]);
                            Console.WriteLine("Key saved");
                            break;
                        case "getkey":
                            if (args.Length != 2)
                            {
                                throw new SMException("Too many arguments");
                            }
                            var email = args[1];
                            secureMsgClient.GetKey(email);
                            break;
                        case "sendmsg":
                            if (args.Length != 3)
                            {
                                throw new SMException("Missing arguments");
                            }
                            secureMsgClient.SendMsg(args[1], args[2]);
                            Console.WriteLine("Message written");
                            break;
                        case "getmsg":
                            if (args.Length != 2)
                            {
                                throw new SMException("Too many arguments");
                            }
                            Console.WriteLine(secureMsgClient.GetMsg(args[1]));
                            break;
                        default:
                            PrintHelp();
                            break;
                    }
                } catch (SMException sme)
                {
                    Console.WriteLine(sme.Message);
                    PrintHelp();
                }
            } else
            {
                PrintHelp();
            }
        }

        /// <summary>
        ///     Displays a help message with available command instructions.
        /// </summary>
        private static void PrintHelp() {
            Console.WriteLine(
                "Usage: <command> <other arguments>\n" +
                "Commands:\n" +
                "- keyGen <keysize>\n" +
                "\t- \"keysize\" is the number of bits in the key.\n\tThis must be at least 1024 bits and a multiple of 8.\n" +
                "- sendKey <email>\n" +
                "\t- \"email\" is the email of the user whose public \n\t  key is being sent to the server.\n" +
                "- getKey <email>\n" +
                "\t- \"email\" is the email of a user whose public \n\t  key is being retreived from the server.\n" +
                "- sendMsg <email> <plaintext>\n" +
                "\t- \"email\" is the email of the user being sent a \n\t  message.\n" +
                "\t- \"plaintext\" is the message being sent.\n" +
                "- getMsg <email>\n" +
                "\t- \"email\" is the email of the user whose message \n\t  is being retrieved.");
        }
    }
}
