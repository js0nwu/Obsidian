﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FervorLibrary
{
    public class Library
    {
        public string Greeting(string name, int index)
        {
            System.IO.StreamReader greetreader = new System.IO.StreamReader("greet.bin");
            string[] list = greetreader.ReadToEnd().Split('~');
            string greetslist = list[0];
            string languageslist = list[1];
            string[] greets = greetslist.Split(',');
            string[] languages = languageslist.Split(',');

            int greetslength = greets.Length;
            int languageslength = languages.Length;
            string greetlang = greets[index] + " " + name + "! You learned how to greet in " + languages[index];
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
