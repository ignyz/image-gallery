using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixProGallery
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //using (Process p = new Process())
            //{
            //    p.StartInfo = new ProcessStartInfo("myexe.exe");
            //    p.Start();

               
            //}
            Application.Run(new Form2());
            //Application.Run(new Form1());
        }
    }
}
