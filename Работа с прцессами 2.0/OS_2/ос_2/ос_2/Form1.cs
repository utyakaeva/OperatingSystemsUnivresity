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

namespace ос_2
{
    public partial class Form1 : Form
    {
    //    System.Windows.Forms.Timer timerRestarter;
        string[] args;
        Form1 main;
        public RegistryKey Reestr;
        bool sost_processov = false;
        bool sost_reestra = false;
        bool restartSlaves = false;

        public string LogsLocation;// 0 - file, 1 - regisrty, 2 -off
        public string proc_name;

        public string regKeyName;
        public string valueName;
        public string newValueName;
        public string delValue;
      
        public Form1(string[] argss)
        {
            this.main = this;
            this.args = argss;
            InitializeComponent();
            

        }
      


        private void Form1_Load(object sender, EventArgs e)
        {
          //  MyClass();
            razdelVReestre();//cоздание разделов реестре
            GetArgs();//чтение аргументов запуска
          avtozapusk();//пропись в автозапуск
            Thread processMonitor = new Thread(new ThreadStart(monitoringSl));
            processMonitor.IsBackground = true;
            processMonitor.Start();
            Thread statusMonitor = new Thread(new ThreadStart(statussMonitor));
            statusMonitor.IsBackground = true;
            statusMonitor.Start();
            Thread.Sleep(1000);
           
            if (restartSlaves)
            {
                Thread Restart = new Thread(new ThreadStart(RestartSlaves));
                Restart.IsBackground = true;
                Restart.Start();

            }
        }
      
     
        private void button4_Click(object sender, EventArgs e)
        {
           
            if (!sost_processov)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"Proc.exe";
              
                //startInfo.Arguments = regKeyName + " " + valueName + " " + newValueName + " " + delValue + " " + LogsLocation;
                startInfo.Arguments = proc_name+ " 0";
                Process.Start(startInfo);
            }
            else
            {
                Process[] proc = Process.GetProcessesByName("Proc");
                foreach (Process process in proc)
                {
                    process.Kill();
                }
                WriteLogs("Proc");
            }
        }


        private void RestartSlaves()
        {
            do
            {
                if (!sost_processov)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"Proc.exe";
                    startInfo.Arguments = proc_name + " " + LogsLocation;
                    Process.Start(startInfo);
                }
                else
                {
                    Process[] proc = Process.GetProcessesByName("Proc");
                    foreach (Process process in proc)
                    {
                        process.Kill();
                    }
                    WriteLogs("Proc");
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"Proc.exe";
                    startInfo.Arguments = proc_name + " " + LogsLocation;
                    Process.Start(startInfo);
                }
                if (!sost_reestra)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"Реестр.exe";
                    startInfo.Arguments = regKeyName + " " + valueName;
                    Process.Start(startInfo);
                }
                else
                {
                    Process[] proc = Process.GetProcessesByName("Реестр");
                    foreach (Process process in proc)
                    {
                        process.Kill();
                    }
                    WriteLogs("Реестр");
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"Реестр.exe";
                    
                        startInfo.Arguments = regKeyName + " " + valueName;
                    Process.Start(startInfo);
                }
                Thread.Sleep(6000);
            }
            while (true);
        }

        private void WriteLogs(string v)
        {
            if (LogsLocation == "0")
            {
                FileStream bFile = new FileStream("Главный.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(bFile, Encoding.Default);
                StreamReader sr = new StreamReader(bFile, Encoding.Default);
                string StrLine = sr.ReadLine();
                while (StrLine != null)
                {
                    StrLine = sr.ReadLine();
                }
                sw.WriteLine("[" + DateTime.Now.ToString() + "]  Процесс уничтожен - " + v);
                sw.Close();
            }
            if (LogsLocation == "1")
            {
                RegistryKey Reg = Registry.CurrentUser;
                Reg = Reg.OpenSubKey("CLSID", true);
                Reg = Reg.OpenSubKey("Master", true);
                Reg = Reg.OpenSubKey("Логи", true);
                Reg.SetValue(DateTime.Now.ToString(), " Процесс уничтожен " + v);
            }
        }

        private void statussMonitor()
        {
            do
            {
                if (sost_reestra)
                {
                    main.SetText("Активен", label3);
                }
                else
                    main.SetText("Не активен", label3);
                if (sost_processov)
                {
                    main.SetText("Активен", label4);
                }
                else
                    main.SetText("Не активен", label4);

                Thread.Sleep(100);
            }
            while (true);
        }

        private void SetText(string v, Label label3)
        {
            label3.Invoke(new Action(() => { label3.Text = v; }));
        }

        private void monitoringSl()
        {
            do
            {
                Process[] ProcMon = Process.GetProcessesByName("Proc");
                if (ProcMon.Length != 0)
                {
                    sost_processov = true;
                    button1.Invoke(new Action(() => { button4.Text = "выкл"; }));
                    button2.Invoke(new Action(() => { main.button2.Enabled = false; }));
                }
                else
                {
                    sost_processov = false;
                    button1.Invoke(new Action(() => { button4.Text = "вкл"; }));
                    button2.Invoke(new Action(() => { main.button2.Enabled = true; }));
                }
                Process[] RegMon = Process.GetProcessesByName("Реестр");
                if (RegMon.Length != 0)
                {
                    sost_reestra = true;
                    button1.Invoke(new Action(() => { button1.Text = "выкл"; }));
                    button2.Invoke(new Action(() => { main.button3.Enabled = false; }));
                }
                else
                {
                    sost_reestra = false;
                    button1.Invoke(new Action(() => { button1.Text = "вкл"; }));
                    button2.Invoke(new Action(() => { main.button3.Enabled = true; }));
                }
                Thread.Sleep(1000);
            }
            while (true);
        }


       
        private void razdelVReestre()//пространство программы в реестре
        {
            Reestr = Registry.CurrentUser;
            Reestr = Reestr.OpenSubKey("CLSID", true);
            RegistryKey RK = Reestr.OpenSubKey("Master", true);
            if (RK == null)
            {
                Reestr.CreateSubKey("Master");
                RK = Reestr.OpenSubKey("Master", true);
                RK.SetValue("Расположение журналов", "0");
            }
            Reestr = RK;
            Reestr.CreateSubKey("Логи");
            LogsLocation = RK.GetValue("Расположение журналов").ToString();
            if (Convert.ToInt32(LogsLocation) == 1)
                checkBox1.Checked = true;
            foreach (string arg in args)
            {
                if (arg == "-логи")
                {
                    LogsLocation = "2";
                    checkBox1.Enabled = false;
                    checkBox1.Visible = false;
                }
                if (arg == "-перезапуск")
                {
                    restartSlaves = true;
                }
            }
        }
        private void GetArgs()//получение параметров из реестра
        {
            RegistryKey RK = Reestr.OpenSubKey("Proc", true);
            if (RK == null)
            {
                Reestr.CreateSubKey("Proc");
                RK = Reestr.OpenSubKey("Proc", true);
                RK.SetValue("WindowName", "Калькулятор");
               // RK.CreateSubKey("Логи");
            }
            proc_name = RK.GetValue("WindowName").ToString();
            RK = Reestr.OpenSubKey("Реестр", true);
            if (RK == null)
            {
                Reestr.CreateSubKey("Реестр");
                RK = Reestr.OpenSubKey("Реестр", true);
                RK.SetValue("regKeyName", "CLSID");
                RK.SetValue("valueName", "suicid");
                RK.SetValue("newValueName", "");
                RK.SetValue("delValue", "1");
              //  RK.CreateSubKey("Логи");
            }
            regKeyName = RK.GetValue("regKeyName").ToString();
            valueName = RK.GetValue("valueName").ToString();
            newValueName = RK.GetValue("newValueName").ToString();
            delValue = RK.GetValue("delValue").ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!sost_reestra)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"Реестр.exe";
                startInfo.Arguments = regKeyName + " " + valueName + " " + newValueName + " " + delValue + " " + LogsLocation;
                Process.Start(startInfo);
            }
            else
            {
                Process[] proc = Process.GetProcessesByName("Реестр");
                foreach (Process process in proc)
                {
                    process.Kill();
                }
                WriteLogs("Реестр");
            }

        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 PP = new Form2(this);
            PP.ShowDialog();
          
      
    }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                LogsLocation = "1";
                Reestr.SetValue("Расположение журналов", "1");
            }
            else
            {
                LogsLocation = "0";
                Reestr.SetValue("Расположение журналов", "0");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 PP = new Form3(this);
            PP.ShowDialog();

           // PP.Visible = false;

        }
        private void avtozapusk ()
        {
         const string name = "Master";
            string ExePath = Application.ExecutablePath;
           RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            reg.SetValue(name, ExePath);
            reg.Flush();
            reg.Close();

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}







