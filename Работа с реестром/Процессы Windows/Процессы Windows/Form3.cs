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

namespace ос_2
{
    public partial class Form3 : Form
    {

        Form1 main;
        public Form3(Form1 fr1)
        {
            this.main = fr1;
            InitializeComponent();

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
            }
            else
                textBox3.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
            }
            else
                textBox3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            main.regKeyName = textBox1.Text;
            main.valueName = textBox2.Text;
            main.newValueName = textBox3.Text;

            RegistryKey RK = main.Reestr.OpenSubKey("Реестр", true);
            RK.SetValue("regKeyName", textBox1.Text);
            RK.SetValue("valueName", textBox2.Text);
            RK.SetValue("newValueName", textBox3.Text);
            if (radioButton1.Checked)
            {
                RK.SetValue("delValue", "1");
                main.delValue = "1";
            }
            else
            {
                RK.SetValue("delValue", "0");
                main.delValue = "0";
            }

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            textBox1.Text = main.regKeyName;
            textBox2.Text = main.valueName;
            textBox3.Text = main.newValueName;
            if (main.delValue == "0")
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else

            {
                radioButton2.Checked = true;
                radioButton1.Checked = false;
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}




