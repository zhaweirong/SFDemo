using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SFDemo
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                return;

            //处理未捕获的异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo cp = new System.Diagnostics.ProcessStartInfo();
            cp.FileName = Application.ExecutablePath;
            cp.Arguments = "cmd params";
            cp.UseShellExecute = true;
            System.Diagnostics.Process.Start(cp);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo cp = new System.Diagnostics.ProcessStartInfo();
            cp.FileName = Application.ExecutablePath;
            cp.Arguments = "cmd params";
            cp.UseShellExecute = true;
            System.Diagnostics.Process.Start(cp);
        }
    }
}
