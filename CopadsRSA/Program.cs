using CopadsRSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace copadsRSA
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length >=2 || args.Length <= 3)
            {
                var secureMsgClient = new SecureMessaging();
                var command = args[0].ToLower();

                switch (command)
                {
                    case "keygen":
                        break;
                    case "sendkey":
                        break;
                    case "getkey":
                        var email = args[1];
                        secureMsgClient.getKey(email);
                        break;
                    case "sendmsg":
                        break;
                    case "getmsg":
                        break;
                    default:
                        break;
                }
            }

            //Console.ReadKey();
        }
    }
}
