using System;
using System.Windows.Forms;

namespace Scheduling_Desktop_App
{
    internal static class Program
    {
        public static string CurrentUser { get; set; }
        public static int CurrentUserId { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DatabaseConnection.StartConnection();
            Application.Run(new LoginForm());
            DatabaseConnection.StopConnection();
        }
    }
}
