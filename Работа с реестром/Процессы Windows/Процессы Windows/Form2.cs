using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ос_2
{
    public partial class Form2 : Form
    {
        Form1 main;

        public Form2(Form1 fr1)
        {
            this.main = fr1;
            InitializeComponent();
        }
       

      

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = main.proc_name;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {   
           
            main.proc_name = textBox1.Text;
         RegistryKey RK = main.Reestr.OpenSubKey("Proc2", true);
       RK.SetValue("WindowName", textBox1.Text);
        
            //Process proc = Process.GetProcessesByName(textBox1.Text)[0];
             //proc.Kill();
          
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
    

