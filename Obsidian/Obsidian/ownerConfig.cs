using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ObsidianFunctions; 

namespace Obsidian
{
    public partial class ownerConfig : Form
    {
        public ownerConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string ownername = textBox1.Text;
            string ownerpass = textBox2.Text;
            StreamReader sr1 = new StreamReader("users.bin");
            string oldusers = sr1.ReadToEnd();
            sr1.Close();
            StreamReader sr2 = new StreamReader("passwords.bin");
            string oldpasses = sr2.ReadToEnd();
            sr2.Close(); 
            StreamWriter sw = new StreamWriter("owner.bin");
            sw.Write(ownername);
            sw.Close();
            StreamWriter sw2 = new StreamWriter("users.bin");
            sw2.Write(oldusers + ownername + ":");
            sw2.Close();
            StreamWriter sw3 = new StreamWriter("passwords.bin");
            Functions ObsidFunc = new Functions();
            string passhash = ObsidFunc.md5calc(ownerpass);
            sw3.Write(oldpasses + passhash + ":");
            sw3.Close(); 
            this.Hide();
        }

    }
}
