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
				System.IO.StreamReader sr = new System.IO.StreamReader(".activeusers");
                            string[] users = sr.ReadToEnd().Split(':');
                            sr.Close();
                            foreach (string x in users)
                            {
                                if (x.Contains(rnick))
                                {
                                    string query = rmsg.Remove(0, 12);
                                    ObsidianFunctions.Functions obsidfunc = new ObsidianFunctions.Functions();
                                    string list = obsidfunc.removeUser(query);
                                    Console.WriteLine("PRIVMSG " + channel + " :" + list);
                                }
                            }
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}