using RTSS;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace YAKD.Utils
{
    public static class RivaTuner
    {
        public static string rtss_exe = @"C:\Program Files (x86)\RivaTuner Statistics Server\RTSS.exe";
        
        public static string LabelColor;
        public static string ColorBad;
        public static string ColorMid;
        public static string ColorGood;
        public static Process RTSSInstance;
        static OSD osd;

        public static uint chartOffset = 0;


        public static void Print(string text)
        {
            osd.Update(text);
        }

        static RivaTuner()
        {
            if (!IsRivaRunning())
            {
                RunRiva();
            }
            else
            {
                osd = new OSD("customRTSS");
            }
        }

        public static bool IsRivaRunning()
        {
            Process[] pname = Process.GetProcessesByName("RTSS");
            // TODO: short
            if (pname.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool VerifyRiva()
        {
           return File.Exists(rtss_exe);
        }

        public static void RunRiva()
        {
            FileInfo rtssFile = new FileInfo(rtss_exe);
            if (VerifyRiva())
            {
                try
                {
                    RTSSInstance = Process.Start(rtssFile.FullName);
                    Thread.Sleep(2000);
                }
                catch (Exception)
                {
                    throw;
                }
            }
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
                Process[] proc = Process.GetProcessesByName("RTSSHooksLoader64");
                proc[0].Kill();
            }
            catch (Exception) { }            
        }

        public static string TextFormat()
        {
            // TODO: short
            return "<C0=" + LabelColor + "><C1=" + ColorBad+ "><C2=" + ColorMid + "><C3=" + ColorGood + "><S0=47><S1=65><S2=55><A0=-15><A1=55>";
        }        

        public static void BuildRivaOutput()
        {
            string output = "";
            //if(App.meterState.TickRate == 0 && App.meterState.Game == "")
            //{
            //    PrintData(output, true);
            //    return;
            //}
            chartOffset = 0;
           
            PrintData(output, true);
        }

        public static void PrintData(string text, bool RunRivaFlag = false)
        {
            if ((!IsRivaRunning() && !RunRivaFlag) || !VerifyRiva()) return;
           
            
            if (!IsRivaRunning() && RunRivaFlag)
            {
                RunRiva();
            }
            if (text != "")
            {
                text = TextFormat() + text;
            }
            Print(text);
        }
    }
}
