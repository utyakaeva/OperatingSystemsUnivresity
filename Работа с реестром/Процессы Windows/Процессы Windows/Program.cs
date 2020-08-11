using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ос_2
{
    static class Program
    {
        private static Mutex _syncobject;
        private const string _syncobjectName = "Master";
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            bool createdNew;
            _syncobject = new Mutex(true, _syncobjectName, out createdNew);
            if (!createdNew)
            {
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(args));
            }
        }
    }
}
