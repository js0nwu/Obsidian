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
				System.IO.StreamReader sr2 = new System.IO.StreamReader(".activeusers");
				string old = sr2.ReadToEnd();
				sr2.Close();
				System.IO.StreamReader sr = new System.IO.StreamReader("users.bin");
				string[] registeredusers = sr.ReadToEnd().Split(':');
				sr.Close();
				foreach (string x in registeredusers)
				{
					if (x == rnick)
					{
						string query = rmsg.Remove(0, 8);
						int indexuser = Array.IndexOf(registeredusers, rnick);
						ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
						bool passcorrect = ObsidFunc.isVerified(indexuser, query);
						System.IO.StreamWriter sw = new System.IO.StreamWriter(".activeusers");
						if (passcorrect == true)
						{
							sw.Write(old + rnick + ":");
							Console.WriteLine("PRIVMSG " + rnick + " :Success! You are logged in!");
						}
						sw.Close(); 
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