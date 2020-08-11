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

namespace oc1
{
    public partial class Form1 : Form
    {
        public TreeNode uzel_current;
        public TreeNode Slctd;
        public RegistryKey Temp;
        public RegistryKey SlctdRK;
        public RegistryKey parentNode;
        public string slctdname;
        public string name;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Visible = true;
            //treeView1.Visible = false;
            //dataGridView1.Visible = false;
            //button1.Visible = false;

        }
        public void derevo(RegistryKey RK)//заполнение 
        {
            String[] S = RK.GetSubKeyNames();
            int f = 0;
            foreach (string str in S)
            {
                bool check = true;
                try
                {
                    RegistryKey n1 = RK.OpenSubKey(str);//проверка доступа
                }
                catch (Exception)
                {
                    check = false;
                }
                if (check)
                {
                    uzel_current.Nodes.Add(str);
                    uzel_current = uzel_current.Nodes[f];
                    RegistryKey n1 = RK.OpenSubKey(str);
                    derevo(n1);
                    uzel_current = uzel_current.Parent;
                }
                else
                {
                    uzel_current.Nodes.Add(str);
                }
                f++;
                if (uzel_current == null) uzel_current = treeView1.Nodes[0];

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Add("HKEY_CURRENT_USER");
            RegistryKey n1 = Registry.CurrentUser;
            uzel_current = treeView1.Nodes[0];
            derevo(n1);
            treeView1.Nodes.Add("HKEY_CURRENT_CONFIG");
            RegistryKey n2 = Registry.CurrentConfig;
            uzel_current = treeView1.Nodes[1];
            derevo(n2);
           // button2.Visible = false;//активация интерфейса

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dataGridView1.Rows.Clear();
            Slctd = treeView1.SelectedNode;
            string adr = Slctd.Text;
            TreeNode tr = Slctd;
            slctdname = treeView1.SelectedNode.Text;
            while (tr.Parent != null)
            {
                adr = tr.Parent.Text + '/' + adr;
                tr = tr.Parent;
            }
            string[] adrs = adr.Split('/');
            RegistryKey rgk;
            if (adrs[0] == "HKEY_CURRENT_USER")
            {
                rgk = Registry.CurrentUser;
            }
            else
            {
                rgk = Registry.CurrentConfig;
            }
            bool a = true;
            try
            {
                RegistryKey rgk1 = rgk.OpenSubKey(adrs[1], true);//проверка доступа
            }
            catch (Exception)
            {
                a = false;
            }
            if (a)
            {
                RegistryKey rgk1 = rgk.OpenSubKey(adrs[1], true);
                int i = 0;
                if (adrs != null)
                {
                    foreach (string A in adrs)
                    {

                        button1.Enabled = true;

                        if (i > 1)
                        {
                            try
                            {
                                parentNode = rgk1;
                                rgk1 = rgk1.OpenSubKey(A, true); //проверка доступа
                            }
                            catch (Exception)
                            {
                                button1.Enabled = false;
                                parentNode = rgk1;
                                rgk1 = rgk1.OpenSubKey(A);
                            }
                        }
                        i++;
                    }
                }
                string[] parametrs = rgk1.GetValueNames();
                if (parametrs != null)
                {
                    foreach (string p in parametrs)
                    {
                        dataGridView1.Rows.Add(p, rgk1.GetValue(p).ToString());
                    }
                }
                SlctdRK = rgk1;
            }
            else
            {
                button1.Enabled = false;

            }

        }
     

        private void button1_Click(object sender, EventArgs e)
        {
            parentNode.DeleteSubKey(name);
            Slctd.Remove();
        }

        private void button3_Click(object sender, EventArgs e)
        {
          
            Temp = SlctdRK;
            name = slctdname;
            button1.Enabled = true;
        }

       
        public void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string paramName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(); // имя параметра
                string paramValue = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(); // значение параметра
                SlctdRK.SetValue(paramName, paramValue);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("HKEY_CURRENT_USER");
            RegistryKey n1 = Registry.CurrentUser;
            uzel_current = treeView1.Nodes[0];
            derevo(n1);
            treeView1.Nodes.Add("HKEY_CURRENT_CONFIG");
            RegistryKey n2 = Registry.CurrentConfig;
            uzel_current = treeView1.Nodes[1];
            derevo(n2);
            button2.Enabled = false;
           

        }



        private void button5_Click_1(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Slctd = treeView1.SelectedNode;
            string adr = Slctd.Text;
            TreeNode tr = Slctd;
            slctdname = treeView1.SelectedNode.Text;
            while (tr.Parent != null)
            {
                adr = tr.Parent.Text + '/' + adr;
                tr = tr.Parent;
            }
            string[] adrs = adr.Split('/');
            RegistryKey rgk;
            if (adrs[0] == "HKEY_CURRENT_USER")
            {
                rgk = Registry.CurrentUser;
            }
            else
            {
                rgk = Registry.CurrentConfig;
            }
            bool a = true;
            try
            {
                RegistryKey rgk1 = rgk.OpenSubKey(adrs[1], true);//проверка доступа
            }
            catch (Exception)
            {
                a = false;
            }
            if (a)
            {
                RegistryKey rgk1 = rgk.OpenSubKey(adrs[1], true);
                int i = 0;
                if (adrs != null)
                {
                    foreach (string A in adrs)
                    {

                        button1.Enabled = true;

                        if (i > 1)
                        {
                            try
                            {
                                parentNode = rgk1;
                                rgk1 = rgk1.OpenSubKey(A, true); //проверка доступа
                            }
                            catch (Exception)
                            {
                                button1.Enabled = false;
                                parentNode = rgk1;
                                rgk1 = rgk1.OpenSubKey(A);
                            }
                        }
                        i++;
                    }
                }
                string[] parametrs = rgk1.GetValueNames();
                if (parametrs != null)
                {
                    foreach (string p in parametrs)
                    {
                        dataGridView1.Rows.Add(p, rgk1.GetValue(p).ToString());
                    }
                }
                SlctdRK = rgk1;
            }
            else
            {
                button1.Enabled = false;

            }

        }
    }

       
    }




    


