using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notion2TistoryConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FileWatcher watcher = new FileWatcher();
            watcher.InitWatcher();
            /*
            Console.Write("Client id     : ");
            string clientId = Console.ReadLine();
            Console.Write("Secret Key    : ");
            string clientSK = Console.ReadLine();
            Console.Write("Redirect url  : ");
            string redirect = Console.ReadLine();
            Console.Write("User id       : ");
            string userID = Console.ReadLine();
            Console.Write("User password : ");
            string userPW = Console.ReadLine();
            Console.Write("Blog name     : ");
            string blogName = Console.ReadLine();

            TistoryAPI client = new TistoryAPI(clientId, clientSK, redirect, userID, userPW, blogName);
            Content testcontent = new Content("test", "<h1>test!</h1>");
            //client.WritePost(testcontent).Wait();
            */
        }
    }
}