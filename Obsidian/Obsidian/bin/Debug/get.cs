using System;
using FervorLibrary;
using ObsidianFunctions;
using System.Collections.Generic; 

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
			string query = rmsg.Remove(0, 5);
if (query.StartsWith("me "))
{
string query2 = query.Remove(0, 3);
query = rnick + " " + query2;

}
Console.WriteLine("PRIVMSG " + channel + " :" + "\u0001ACTION " + "gives " + query + "\u0001"); 
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}