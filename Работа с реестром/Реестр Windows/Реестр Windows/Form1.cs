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

namespace Реестр
{
    public partial class Form1 : Form
    {
       
        string[] args;

        RegistryKey reg = Registry.CurrentUser;
        string name = "";
        private static Mutex _syncobject;
        private const string _syncobjectName = "Реестр";
        public Form1(string[] argss)
        {
            args = argss;
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

            if (args.Length != 0)
            {
                reg = reg.OpenSubKey(args[0]);
                if (reg == null)
                {
                    reg = Registry.CurrentUser;
                    name = args[0];
                }
                else
                {
                    name = args[1];
                }
                Thread RegMonitor = new Thread(new ThreadStart(reestrMonitor));
                RegMonitor.IsBackground = true;
                RegMonitor.Start();

                Thread MasterStat = new Thread(new ThreadStart(masterst));
                MasterStat.IsBackground = true;
                MasterStat.Start();
            }
        }
        private void masterst()
        {
            do
            {
                Mutex _syncMaster;
                const string _syncobjectName = "Master";
                _syncMaster = new Mutex(false, _syncobjectName);
                if (_syncMaster.WaitOne(1))//
                {
                    _syncMaster.ReleaseMutex();
                    WriteFile("Master's не существует - Реестр не доступен");
                    Application.Exit();
                }
                Thread.Sleep(200);
            }
            while (true);
        }

        private void reestrMonitor()
        {
            do
            {
                Find(reg, name);
                Thread.Sleep(5000);
            }
            while (true);
        }

        private void Find(RegistryKey RegKey, string ParamName)
        {
            foreach (string key in RegKey.GetSubKeyNames())
            {
                try
                {
                    Find(RegKey.OpenSubKey(key, true), ParamName);
                }
                catch
                {
                }
            }
            foreach (string val in RegKey.GetValueNames())
            {
                if (val == ParamName)
                {
                    if (Convert.ToInt32(args[args.Length - 2]) == 1)
                    {

                        RegKey.DeleteValue(val);
                        MessageBox.Show("Параметр удалён");

                        if (Convert.ToInt32(args[args.Length - 1]) == 0)
                        {
                            WriteFile(args[1] + " Удален из " + RegKey.Name);
                        }
                        if (Convert.ToInt32(args[args.Length - 1]) == 1)
                        {
                            WriteReg(args[1] + " Удален из " + RegKey.Name);
                        }
                    }
                    else
                    {
                        RegKey.SetValue(args[2], RegKey.GetValue(val));
                        RegKey.DeleteValue(val);
                        MessageBox.Show("Параметр изменён на " + args[2]);

                        if (Convert.ToInt32(args[args.Length - 1]) == 0)
                        {
                            WriteFile(args[1] + " изменено на " + args[2] + " в " + RegKey.Name);
                        }
                        if (Convert.ToInt32(args[args.Length - 1]) == 1)
                        {
                            WriteReg(args[1] + " изменено " + args[2] + " в " + RegKey.Name);
                        }
                    }
                }
            }
        }

        private void WriteFile(string data)
        {
            Mutex SyncWriter = new Mutex(true, "Writer");
            SyncWriter.WaitOne();
            FileStream bFile = new FileStream("Главный.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(bFile, Encoding.Default);
            sw.WriteLine("[" + DateTime.Now.ToString() + "] " + data);
            sw.Close();
            SyncWriter.ReleaseMutex();
        }

        private void WriteReg(string data)
        {
            Mutex SyncWriter = new Mutex(true, "Writer");

            SyncWriter.WaitOne();
            RegistryKey Reg = Registry.CurrentUser;
            Reg = Reg.OpenSubKey("CLSID", true);
            Reg = Reg.OpenSubKey("Master", true);
            Reg = Reg.OpenSubKey("Реестр", true);
            Reg = Reg.OpenSubKey("Логи", true);
            if (Reg == null)
            {
                Reg.CreateSubKey("Логи");
                Reg = Reg.OpenSubKey("Логи", true);
            }
            Reg.SetValue(DateTime.Now.ToString(), data);
            SyncWriter.ReleaseMutex();
        }





        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }

    }
}