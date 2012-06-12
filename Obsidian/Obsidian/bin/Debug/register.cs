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
				string query = rmsg.Remove(0, 10);
				ObsidianFunctions.Functions requestreg = new ObsidianFunctions.Functions();
				string pass = requestreg.reqreguser(rnick, query);
				Console.WriteLine("PRIVMSG " + rnick + " :You have been requested registration - user=" + rnick + " password=" + pass + "(hashed)"); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}