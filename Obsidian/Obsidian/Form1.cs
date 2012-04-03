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
        System.IO.TextReader input;
        System.IO.TextWriter output;

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
            send("MODE " + nick + " +B");
            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
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
            if (System.IO.File.Exists("!rss.txt") == false)
            {
                StreamWriter sw = new StreamWriter("!rss.txt");
                sw.Write("RSS feed not set!");
                sw.Close();
            }
            if (System.IO.File.Exists("!home.txt") == false)
            {
                StreamWriter sw = new StreamWriter("!home.txt");
                sw.Write("Home Page not set!");
                sw.Close();
            }
            if (System.IO.File.Exists("!wiki.txt") == false)
            {
                StreamWriter sw = new StreamWriter("!wiki.txt");
                sw.Write("Wiki link not set!");
                sw.Close();
            }
            if (System.IO.File.Exists("!youtube.txt") == false)
            {
                StreamWriter sw = new StreamWriter("!youtube.txt");
                sw.Write("Youtube Channel not set!");
                sw.Close();
            }
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
                    string rmsg = tmparr[1];
                    if (rmsg.Contains("!respond") == true)
                    {
                        string response = "PRIVMSG " + textBox3.Text + " :Response";
                        send(response);
                    }
                    mail = rnick + ">" + rmsg;
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "JOIN")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    string rnick = tmparr[0];
                    string greeting = "Greetings, " + rnick;
                    
                    string greetingmessage = "PRIVMSG " + textBox3.Text + " :" + greeting;
                    send(greetingmessage);
                }
                else if (mail.Substring(mail.IndexOf(" ") + 1, 4) == "PART" | mail.Substring(mail.IndexOf(" ") + 1, 4) == "QUIT")
                {
                    string[] tmparr = null;
                    mail = mail.Remove(0, 1);
                    tmparr = mail.Split('!');
                    string rnick = tmparr[0];
                    string farewell = "Farewell, " + rnick;
                    
                    string farewellmessage = "PRIVMSG " + textBox3.Text + " :" + farewell;
                    send(farewellmessage);
                }
            }
            
            return mail;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string mail = recv();
            textBox5.Text = mail;
        }

    }
}
