using RTSSSharedMemoryNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;

namespace YAKD.Utils
{
    public static class RTSSHandler
    {
        public static string RTSSPath { get; set; }
        private static Process RTSSInstance;
        static OSD osd;        

        static RTSSHandler()
        {
            RTSSPath = @"C:\Program Files (x86)\RivaTuner Statistics Server\RTSS.exe";            
        }

        public static void Print(string text)
        {
            if (IsRTSSRunning() && osd != null)
            {
                osd.Update(text);
            }
        }

        public static void RunRTSS()
        {
            if (RTSSInstance == null && !IsRTSSRunning() && File.Exists(RTSSPath))
            {
                try
                {
                    RTSSInstance = Process.Start(RTSSPath);
                    Thread.Sleep(2000); // For what? Idk. If it works, don't touch it
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Could not start the RTSS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                RunOSD();
            }
        }

        public static void RunOSD()
        {
            if (osd == null)
            {
                try
                {
                    osd = new OSD("YAKDOSD");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Could not start the OSD", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static bool IsRTSSRunning()
        {
            return Process.GetProcessesByName("RTSS").Length == 0 ? false : true;
        }

        public static void KillRTSS()
        {
            if (RTSSInstance != null)
            {
                try
                {
                    RTSSInstance.Kill();
                    RTSSInstance = null;
                    Process[] proc = Process.GetProcessesByName("RTSSHooksLoader64");
                    proc[0].Kill();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Failed to close RTSS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}