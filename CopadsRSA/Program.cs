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
                        var numBits = int.Parse(args[1]);
                        secureMsgClient.keyGen(numBits);
                        break;
                    case "sendkey":
                        secureMsgClient.sendKey(args[1]);
                        break;
                    case "getkey":
                        var email = args[1];
                        secureMsgClient.getKey(email);
                        break;
                    case "sendmsg":
                        secureMsgClient.sendMsg(args[1], args[2]);
                        break;
                    case "getmsg":
                        Console.WriteLine(secureMsgClient.getMsg(args[1]));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
