using RTSS;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

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
            RunRTSS();

            osd = new OSD("YAKDOSD");
            osd.Update("");
        }

        public static void Print(string text)
        {
            RunRTSS(); // Just in case
            osd.Update(text);
        }

        public static void RunRTSS()
        {
            if (!IsRTSSRunning())
            {
                FileInfo rtssFile = new FileInfo(RTSSPath);
                if (File.Exists(RTSSPath))
                {
                    try
                    {
                        RTSSInstance = Process.Start(rtssFile.FullName);
                        Thread.Sleep(2000); // For what? Idk
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public static bool IsRTSSRunning()
        {
            return Process.GetProcessesByName("RTSS").Length == 0 ? false : true;
        }

        public static void KillRTSS()
        {
            if (RTSSInstance == null)
            {
                return;
            }
            try
            {
                RTSSInstance.Kill();
                RTSSInstance = null;
                Process[] proc = Process.GetProcessesByName("RTSSHooksLoader64");
                proc[0].Kill();
            }
            catch (Exception) { }
        }
    }
}