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
				ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
				bool nickuser = ObsidFunc.isActiveUser(rnick);
				if (nickuser == true)
				{
					System.IO.StreamReader sr = new System.IO.StreamReader("registers.bin");
					string[] regusersarray = sr.ReadToEnd().Split('|');
					string regusers = regusersarray[0];
					sr.Close();
					if (regusers == "" | regusers == null)
					{
						regusers = "None";
					}
					Console.WriteLine("PRIVMSG " + channel + " :" + regusers);
				}
				else
				{
					Console.WriteLine("PRIVMSG " + channel + " :" + "Insufficient permissions!"); 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("PRIVMSG " + channel + " :" + ex.ToString()); 
			}
        }
    }
}