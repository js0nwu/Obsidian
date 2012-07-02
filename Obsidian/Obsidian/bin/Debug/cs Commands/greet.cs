using System;
using FervorLibrary;
using ObsidianFunctions;

namespace CodeCompile
{
    class Program
    {
        static void Main(string[] args)
        {
            string channel = args[0];
            string rnick = args[1];
            string rmsg = args[2];
            try
            {
                string query = rmsg.Remove(0, 7);
                int queryindex = System.Int32.Parse(query);
                FervorLibrary.Library Greeting = new FervorLibrary.Library();
                string returngreet = Greeting.greet(queryindex);
                string response = "PRIVMSG " + channel + " :" + returngreet;
                System.Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex); 
            }
        }
    }
}