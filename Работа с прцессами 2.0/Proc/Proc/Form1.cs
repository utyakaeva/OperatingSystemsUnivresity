using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proc
{

    public partial class Form1 : Form
    {
        string[] args;

        public Form1(string[] argss)
        {
            this.args = argss;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string s = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                s = s + args[i] + " ";
            }
            string str = s.Substring(0, s.Length - 1);
            args[0] = str;
            Thread processMonitor = new Thread(new ThreadStart(procMonitor));
            processMonitor.IsBackground = true;
            processMonitor.Start();
        }
        private void procMonitor()
        {
            do
            {
                Process[] Processes = Process.GetProcesses();
                if (args.Length != 0)
                {
                    foreach (Process process in Processes)
                    {
                        if (process.ProcessName== args[0])
                        {
                            process.Kill();
                            if (Convert.ToInt32(args[args.Length - 1]) == 0)
                            {
                                FileStream bFile = new FileStream("Главный.txt", FileMode.OpenOrCreate);
                                StreamWriter sw = new StreamWriter(bFile, Encoding.Default);
                                StreamReader sr = new StreamReader(bFile, Encoding.Default);
                                string StrLine = sr.ReadLine();
                                while (StrLine != null)
                                {
                                    StrLine = sr.ReadLine();
                                }
                                sw.WriteLine("[" + DateTime.Now.ToString() + "]" + " Завершён процесс с именем окна <<" + args[0] + ">>");
                                sw.Close();
                            }
                            if (Convert.ToInt32(args[args.Length - 1]) == 1)
                            {
                                RegistryKey Reg = Registry.CurrentUser;
                                Reg = Reg.OpenSubKey("CLSID", true);
                                Reg = Reg.OpenSubKey("Master", true);
                                Reg = Reg.OpenSubKey("Proc", true);
                                Reg = Reg.OpenSubKey("Логи", true);
                                if (Reg == null)
                                {
                                    Reg.CreateSubKey("Логи");
                                    Reg = Reg.OpenSubKey("Логи", true);
                                }
                                Reg.SetValue(DateTime.Now.ToString(), args[0] + " " + "Closed");
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
            while (true);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }

       
    }
}


