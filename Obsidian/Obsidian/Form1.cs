using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using ObsidianFunctions;
using FervorLibrary; 

namespace Obsidian
{
    public partial class Form1 : Form
    {
        int port;
        string buf;
        string nick;
        string owner;
        string server;
        string channel;
        System.Net.Sockets.Socket sock;
        string mail;
        int greetnumber;
        string rmsg;
        int farewellnumber;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string IRCInfo = textBox1.Text + "|" + textBox2.Text + "|" + textBox3.Text + "|" + textBox4.Text;
            System.IO.StreamWriter IRCInfoWrite = new System.IO.StreamWriter("IRCInfo.bin");
            IRCInfoWrite.Write(IRCInfo);
            IRCInfoWrite.Close();
            port = Int32.Parse(textBox2.Text);
            server = textBox1.Text;
            channel = textBox3.Text;
            nick = textBox4.Text;
            owner = "Obsidian";
            System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(server);
            System.Net.IPEndPoint EP = new System.Net.IPEndPoint(ipHostInfo.AddressList[0], port);
            sock = new System.Net.Sockets.Socket(EP.Address.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            sock.Connect(server, port);
            send("NICK " + nick);
            send("USER " + nick + " 0 * :FervorBot");
            send("JOIN " + channel);
            send("MODE " + nick + " +B");;
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadIRCInfo();
            Thread configGreet = new Thread(GreetConfig);
            configGreet.Start();
            Thread configFarewell = new Thread(FarewellConfig);
            configFarewell.Start();
        }

        public void send(string msg)
        {
            msg += "\r\n";
            Byte[] data = System.Text.ASCIIEncoding.UTF8.GetBytes(msg);
            sock.Send(data, msg.Length, System.Net.Sockets.SocketFlags.None);
        }
        public string recv()
        {
            byte[] data = new byte[3072];
            sock.Receive(data, 3072, System.Net.Sockets.SocketFlags.None);
            string mail = System.Text.ASCIIEncoding.UTF8.GetString(data);
            if (mail.Contains(" "))
            {
                if (mail.Substring(0, 4) == "PING")
                {
                    string pserv = mail.Substring(mail.IndexOf(":"), mail.Length - mail.IndexOf(":"));
                    pserv = pserv.TrimEnd((char)0);
                    mail = "PING from " + pserv + "\r" + "PONG to " + pserv;
                    send("PONG " + pserv);
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 7) == "PRIVMSG")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    string rnick = tmparr[0];
                    tmparr = mail.Split(':');
                    rmsg = tmparr[1];
                    if (rmsg.Contains("!respond") == true)
                    {
                        string response = "PRIVMSG " + textBox3.Text + " :Response";
                        send(response);
                    }
                    if (rmsg.Contains("!greet "))
                    {
                        string query = rmsg.Remove(0, 7);
                        try
                        {
                            int queryindex = Int32.Parse(query);
                            if (queryindex <= greetnumber)
                            {
                                FervorLibrary.Library Greeting = new FervorLibrary.Library();
                                string returngreet = Greeting.greet(queryindex);

                                string response = "PRIVMSG " + channel + " :" + returngreet;
                                send(response);
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :I don't know that many languages!");
                            }
                        }
                        catch (Exception ex)
                        {
                            send("PRIVMSG " + channel + " :Something went wrong: " + ex);
                        }

                        
                    }
                    if (rmsg.Contains("!farewell "))
                    {
                        string query = rmsg.Remove(0, 10);
                        try
                        {
                            int queryindex = Int32.Parse(query);
                            if (queryindex <= greetnumber)
                            {
                                FervorLibrary.Library Farewelling = new FervorLibrary.Library();
                                string returnfarewell = Farewelling.farewell(queryindex);
                                string response = "PRIVMSG " + channel + " :" + returnfarewell;
                                send(response);
                            }
                            else
                            {
                                send("PRIVMSG " + channel + " :I don't know that many languages!");
                            }
                        }
                        catch (Exception ex)
                        {
                            send("PRIVMSG " + channel + " :Something went wrong: " + ex);
                        }
                    }
                    if (rmsg.Contains("!md5 "))
                    {
                        Thread md5calc = new Thread(generateMD5message);
                        md5calc.Start();
                    }
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "JOIN")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    string rnick = tmparr[0];
                    FervorLibrary.Library Greetings = new FervorLibrary.Library();
                    Random rand = new Random();
                    int indexgreet = rand.Next(0, greetnumber);
                    string greeting = Greetings.Greeting(rnick, indexgreet);
                    string greetingmessage = "PRIVMSG " + textBox3.Text + " :" + greeting;
                    send(greetingmessage);
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "PART" | mail.Substring(mail.IndexOf(" ") + 1, 4) == "QUIT")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    string rnick = tmparr[0];
                    FervorLibrary.Library Farewells = new FervorLibrary.Library();
                    Random rand = new Random();
                    int indexfarewell = rand.Next(0, farewellnumber);
                    string farewell = Farewells.Farewell(rnick, indexfarewell);
                    string farewellmessage = "PRIVMSG " + textBox3.Text + " :" + farewell;
                    send(farewellmessage);
                }
            }
            
            return mail;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Thread updateirc = new Thread(ircupdate);
            updateirc.Start();
        }
        public void GreetConfig()
        {
            if (System.IO.File.Exists("greet.bin") == false)
            {
                System.IO.StreamWriter greetwrite = new System.IO.StreamWriter("greet.bin");
                greetwrite.Write("Hallo,Ahalan,Ni Hao,Ahoj,Goddag,Goede dag,Hello,Bonjour,Guten Tag,Aloha,Shalom,Namaste,Dia dhuit,Ciao,Kon-nichiwa,Zdravstvuyte,Hola,Hej,Sawubona~");
                greetwrite.Write("Afrikaans,Arabic,Chinese,Czech,Danish,Dutch,English,French,German,Hawaiian,Hebrew,Hindi,Irish,Italian,Japanese,Russian,Spanish,Swedish,Zulu");
                greetwrite.Close();
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                greetnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("greet.bin") == true)
            {
                System.IO.StreamReader greetread = new System.IO.StreamReader("greet.bin");
                string greetraw = greetread.ReadToEnd();
                string[] split = greetraw.Split(',');
                greetnumber = split.Length / 2;
            }
        }
        public void FarewellConfig()
        {
            if (System.IO.File.Exists("farewell.bin") == false)
            {
                System.IO.StreamWriter farewellwrite = new System.IO.StreamWriter("farewell.bin");
                farewellwrite.Write("Tosiens,Ma'a as-salaama,Zai Jian,Sbohem,Farvel,Afscheid,Goodbye,Au revoir,Aurf Wiedersehen,Aloha,Kol Tuv,Ach-ha,Slan agat,Addio,Sayonara,Pa-ka,Adios,Adjo,Hamba kahle~");
                farewellwrite.Write("Afrikaans,Arabic,Chinese,Czech,Danish,Dutch,English,French,German,Hawaiian,Hebrew,Hindi,Irish,Italian,Japanese,Russian,Spanish,Swedish,Zulu");
                farewellwrite.Close();
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                farewellnumber = split.Length / 2;
            }
            else if (System.IO.File.Exists("farewell.bin") == true)
            {
                System.IO.StreamReader farewellread = new System.IO.StreamReader("farewell.bin");
                string farewellraw = farewellread.ReadToEnd();
                string[] split = farewellraw.Split(',');
                farewellnumber = split.Length / 2;
            }
        }
        public void loadIRCInfo()
        {
            if (System.IO.File.Exists("IRCInfo.bin") == true)
            {
                System.IO.StreamReader IRCInfoRead = new System.IO.StreamReader("IRCInfo.bin");
                string IRCInfo = IRCInfoRead.ReadToEnd();
                string[] IRCInfoSplit = IRCInfo.Split('|');
                textBox1.Text = IRCInfoSplit[0];
                textBox2.Text = IRCInfoSplit[1];
                textBox3.Text = IRCInfoSplit[2];
                textBox4.Text = IRCInfoSplit[3];
                IRCInfoRead.Close();

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox5.Text = mail;
        }
        public void ircupdate()
        {
            mail = recv();
        }
        public void generateMD5message()
        {
            string query = rmsg.Remove(0, 5).Replace("\0", "").Trim();
            try
            {
                ObsidianFunctions.Functions ObsidFunc = new ObsidianFunctions.Functions();
                string md5encrypt = ObsidFunc.md5calc(query.ToString()).ToLower();
                string response = "PRIVMSG " + channel + " :" + md5encrypt;
                send(response);
            }
            catch (Exception ex)
            {
                send("PRIVMSG " + channel + " :Something went wrong: " + ex);
            }
        }
    }
}
