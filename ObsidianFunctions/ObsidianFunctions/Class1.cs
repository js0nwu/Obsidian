using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace ObsidianFunctions
{
    public class Functions
    {
        public string md5calc(string input)
        {
            input = input.Replace("\0", "").Trim();
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public string reqreguser(string nickname, string password)
        {
            nickname = nickname.Replace("\0", "").Trim();
            password = password.Replace("\0", "").Trim();
            System.IO.StreamReader sr = new System.IO.StreamReader("registers.bin");
            string[] old = sr.ReadToEnd().Split('|');
            sr.Close();
            string users = old[0];
            string pass = old[1];
            users = users + nickname + ":";
            pass = pass + md5calc(password) + ":";
            System.IO.StreamWriter sw = new System.IO.StreamWriter("registers.bin");
            sw.Write(users + "|" + pass);
            sw.Close();
            return md5calc(password);
        }
        public bool isVerified(int index, string password)
        {
            password = password.Replace("\0", "").Trim();
            password = md5calc(password);
            bool iscorrect;
            StreamReader sr = new StreamReader("passwords.bin");
            string[] allpass = sr.ReadToEnd().Split(':');
            sr.Close();
            string verifypass = allpass[index];
            if (verifypass == password)
            {
                iscorrect = true;
            }
            else
            {
                iscorrect = false; 
            }
            return iscorrect;
        }
        public string addUser(string nickname)
        {
            nickname = nickname.Replace("\0", "").Trim();
            StreamReader sr = new StreamReader("registers.bin");
            string[] registereduserlist = sr.ReadToEnd().Split('|');
            string[] registeredusers = registereduserlist[0].Split(':');
            string[] registeredpasswords = registereduserlist[1].Split(':');
            sr.Close();
            StreamReader sru = new StreamReader("users.bin");
            StreamReader srp = new StreamReader("passwords.bin");
            string oldusers = sru.ReadToEnd();
            string oldpasswords = srp.ReadToEnd();
            sru.Close();
            srp.Close();
            
            foreach (string x in registeredusers)
            {
                if (x.Contains(nickname))
                {
                    int userregIndex = Array.IndexOf(registeredusers, nickname);
                    StreamWriter sw = new StreamWriter("users.bin");
                    sw.Write(oldusers + registeredusers[userregIndex] + ":");
                    StreamWriter sw2 = new StreamWriter("passwords.bin");
                    sw2.Write(oldpasswords + registeredpasswords[userregIndex] + ":");
                    sw.Close();
                    sw2.Close();
                    registeredusers = registeredusers.Where(val => val != nickname).ToArray();
                    registeredpasswords = registeredpasswords.Where(val => val != registeredpasswords[userregIndex]).ToArray();
                    string newuserlist = String.Join(":", registeredusers);
                    string newpasslist = string.Join(":", registeredpasswords);
                    StreamWriter newwrite = new StreamWriter("registers.bin");
                    newwrite.Write(newuserlist + "|" + newpasslist);
                    newwrite.Close();
                }

            }
            StreamReader sr3 = new StreamReader("users.bin");
            string newregusers = sr3.ReadToEnd();
            sr3.Close();
            return newregusers;

        }
    }
}
