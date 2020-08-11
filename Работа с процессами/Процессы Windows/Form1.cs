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

namespace Процессы
{
    public partial class Form1 : Form
    {
        //System.Windows.Forms.Timer timerRestarter;
        string[] args;
        string nameProcess = Environment.GetCommandLineArgs()[0].ToString();
        public Form1(string[] argss)
        {
       
           this.args = argss;
           // InitializeComponent();
            //timerRestarter = new System.Windows.Forms.Timer();
            //timerRestarter.Interval = 5000; //5 секунд
           // timerRestarter.Tick += new EventHandler(timerRestarter_Tick);
           // timerRestarter.Start();

        }
        void timerRestarter_Tick(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath);
            Process.GetCurrentProcess().Kill();
          
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            //string s = "";
            //for (int i = 0; i < args.Length-1; i++)
            //{
            //    s = s + args[i] + " ";
            //}
            //string str = s.Substring(0, s.Length - 1);
            //args[0] = str;
            label1.Text = nameProcess;
            Thread processMonitor = new Thread(new ThreadStart(KillProc));
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
                        if (process.MainWindowTitle == args[0])
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
                                sw.WriteLine("[" + DateTime.Now.ToString() + "]" + " Завершён процесс с именем окна *" + args[0] + "*");
                                sw.Close();
                            }
                            if (Convert.ToInt32(args[args.Length - 1]) == 1)
                            {
                                RegistryKey Reg = Registry.CurrentUser;
                                Reg = Reg.OpenSubKey("CLSID", true);
                                Reg = Reg.OpenSubKey("Master", true);
                                Reg = Reg.OpenSubKey("Процессы", true);
                                Reg = Reg.OpenSubKey("Логи", true);
                                if (Reg == null)
                                {
                                    Reg.CreateSubKey("Логи");
                                    Reg = Reg.OpenSubKey("Логи", true);
                                }
                                Reg.SetValue(DateTime.Now.ToString(), args[0] + " " + "Закрыт");
                            }
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            while (true);
        }

        private void KillProc()
        {
            while (true)
            {
                Process proc = Process.GetProcessesByName(nameProcess)[0];
                proc.Kill();
                RegistryKey Reg = Registry.CurrentUser;
                Reg = Reg.OpenSubKey("CLSID", true);
                Reg = Reg.OpenSubKey("Master", true);
                Reg = Reg.OpenSubKey("Процессы", true);
                Reg = Reg.OpenSubKey("Логи", true);
                if (Reg == null)
                {
                    Reg.CreateSubKey("Логи");
                    Reg = Reg.OpenSubKey("Логи", true);
                }
                Reg.SetValue(DateTime.Now.ToString(), args[0] + " " + "Закрыт");
                Thread.Sleep(1000);
            }
        }
        

        private void Form1_Activated_1(object sender, EventArgs e)
        {
              Visible = true;
        }
    }
}
       
    

