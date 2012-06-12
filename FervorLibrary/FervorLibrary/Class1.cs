using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FervorLibrary
{
    public class Library
    {
        public int greetnumber;
        public int farewellnumber; 
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
        public string Farewell(string name, int index)
        {
            System.IO.StreamReader farewellreader = new System.IO.StreamReader("farewell.bin");
            string[] list = farewellreader.ReadToEnd().Split('~');
            string farewellslist = list[0];
            string languageslist = list[1];
            string[] farewells = farewellslist.Split(',');
            string[] languages = languageslist.Split(',');
            int farewellslength = farewells.Length;
            int languageslength = languages.Length;
            string farewelllang = farewells[index] + " " + name + "! You learned how to say goodbye in " + languages[index];
            return farewelllang;
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
        public string farewell(int index)
        {
            System.IO.StreamReader farewellreader = new System.IO.StreamReader("farewell.bin");
            string[] list = farewellreader.ReadToEnd().Split('~');
            string farewelllist = list[0];
            string languageslist = list[1];
            string[] farewells = farewelllist.Split(',');
            string[] languages = languageslist.Split(',');
            int farewellslist = farewells.Length;
            int languageslength = languages.Length;
            string farewelllang = farewells[index] + "! You learned how to say goodbye in " + languages[index];
            return farewelllang;
        }
        public string reggreet(int index)
        {
            System.IO.StreamReader greetreader = new System.IO.StreamReader("greet.bin");
            string[] list = greetreader.ReadToEnd().Split('~');
            string greetslist = list[0];
            string[] greets = greetslist.Split(',');
            int greetslength = greets.Length;
            string greetlang = greets[index];
            return greetlang;
        }
        public string regfarewell(int index)
        {
            System.IO.StreamReader farewellreader = new System.IO.StreamReader("farewell.bin");
            string[] list = farewellreader.ReadToEnd().Split('~');
            string farewelllist = list[0];
            string[] farewells = farewelllist.Split(',');
            int farewellslist = farewells.Length;
            string farewelllang = farewells[index];
            return farewelllang;
        }
        public bool canGreet()
        {
            if (System.IO.File.Exists("canGreet"))
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        public void greetTrue()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("canGreet");
            sw.Close();
        }
        public void greetFalse()
        {
            if (System.IO.File.Exists("canGreet"))
            {
                System.IO.File.Delete("canGreet");

            }
        }
    }
}
