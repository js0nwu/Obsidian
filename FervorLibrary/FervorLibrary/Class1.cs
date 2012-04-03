using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FervorLibrary
{
    public class Library
    {
        public string Greeting(string name)
        {
            System.IO.StreamReader greetreader = new System.IO.StreamReader("greet.bin");
            string[] list = greetreader.ReadToEnd().Split('~') ;
            string greetslist = list[0];
            string languageslist = list[1];
            string[] greets = greetslist.Split(',');
            string[] languages = languageslist.Split(',');
            int greetslength = greets.Length;
            int languageslength = languages.Length;
            int greetindex = 0;
            int languageindex = 0;
            string greetlang = greets[greetindex] + "~" + languages[languageindex];
            greetindex++;
            languageindex++;
            if (greetindex == greetslength)
            {
                greetindex = 0;
            }
            if (languageindex == languageslength)
            {
                languageindex = 0;
            }
            return greetlang;

        }

        public string greet(int index)
        {
            System.IO.StreamReader greetreader = new System.IO.StreamReader("greet.bin");
            string[] list = greetreader.ReadToEnd().Split('~');
            string greetslist = list[0];
            string languageslist = list[1];
            string[] greets = greetslist.Split(',');
            string[] languages = languageslist.Split(',');
            int greetslength = greets.Length;
            int languageslength = languages.Length;
            string greetlang = greets[index] + "! You learned how to greet in " + languages[index];
            return greetlang;
        }

    }
}
