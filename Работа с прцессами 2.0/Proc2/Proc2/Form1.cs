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

namespace Proc2
{

    public partial class Form1 : Form
    {
        private static Mutex _syncobject;
        private const string _syncobjectName = "Proc2";
        string[] args;

        RegistryKey reg = Registry.CurrentUser;
        public Form1(string[] argss)

        {
            if (argss.Length == 0)
            {
                this.args = new string[2];
                args[0] = "Калькулятор";
                args[1] = "0";
            }
            else
            {
                this.args = argss;
            }
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            bool createdNew;

            _syncobject = new Mutex(true, _syncobjectName, out createdNew);
            if (!createdNew)
            {
                Application.Exit();
            }


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
                        if (process.ProcessName.Contains(args[0])) 
                        {
                            process.Kill();
                            if (Convert.ToInt32(args[args.Length - 1]) == 0)
                            {
                                WriteFile("Завершён процесс с именем окна <<" + args[0] + ">>");
                            }
                            if (Convert.ToInt32(args[args.Length - 1]) == 1)
                            {

                                Mutex SyncWriter = new Mutex(true, "Writer");

                                SyncWriter.WaitOne();
                                RegistryKey Reg = Registry.CurrentUser;
                                Reg = Reg.OpenSubKey("CLSID", true);
                                Reg = Reg.OpenSubKey("Master", true);
                                Reg = Reg.OpenSubKey("Proc2", true);
                                try
                                {
                                    Reg = Reg.OpenSubKey("Логи", true);
                                }
                                catch
                                {
                                    Reg = Reg.CreateSubKey("Логи");
                                }
                                Reg.SetValue(DateTime.Now.ToString(), args[0] + " " + "Closed");
                                SyncWriter.ReleaseMutex();
                            }
                        }
                    }

                }


                //отслеживание мастера
                Mutex _syncMaster;

                const string _syncobjectName = "Master";
                _syncMaster = new Mutex(false, _syncobjectName);
                if (_syncMaster.WaitOne(100))//мимлисекунды
                {
                    _syncMaster.ReleaseMutex();
                    WriteFile("Master's не существует - Proc недоступен");
                    Application.Exit();
                }

                Thread.Sleep(300);
            }
            while (true);
        }
        
        
        private void WriteFile(string data)
        {

            Mutex SyncWriter = new Mutex(true, "Writer");
            SyncWriter.WaitOne();
            FileStream bFile = new FileStream("Главный.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(bFile, Encoding.Default);
            sw.WriteLine("[" + DateTime.Now.ToString() + "] " + data);
            SyncWriter.ReleaseMutex();
            sw.Close();
        }
        
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
        
