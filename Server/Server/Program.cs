using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                ServerLoader.LoadComplete += new EventHandler(ServerLoader_LoadComplete);
                ServerLoader.LoadServer();
                Application.Run();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show("First Chance Exception: " + ((Exception)e.ExceptionObject).ToString());
        }

        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) {
            //MessageBox.Show("First Chance Exception: " + e.Exception.ToString());
        }

        static void ServerLoader_LoadComplete(object sender, EventArgs e) {
            try {
                Application.Run(Globals.MainUI);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
            //Globals.MainUI.Show();
        }
    }
}
