using RTSS;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace tickMeter.Classes
{
    public static class RivaTuner
    {
        public static string rtss_exe = @"C:\Program Files (x86)\RivaTuner Statistics Server\RTSS.exe";
        
        public static string LabelColor;
        public static string ColorBad;
        public static string ColorMid;
        public static string ColorGood;
        public static Process RtssInstance;
        static OSD osd;

        public static uint chartOffset = 0;

        public static string DrawChart(float[] graphData)
        {
            uint chartSize;
            int max = 60;
            if(graphData.Max() > 61)
            {
                max = 90;
            }
            if (graphData.Max() > 91)
            {
                max = 120;
            }
            unsafe
            {
                fixed (float* lpBuffer = graphData)
                {
                    chartSize = osd.EmbedGraph(chartOffset, lpBuffer: lpBuffer, dwBufferPos: 0, 512, dwWidth: -24, dwHeight: -3, dwMargin: 1, fltMin: 0, fltMax: max, dwFlags: 0);
                }
                string chartEntry = "<C1><S2>" + max + "<OBJ=" + chartOffset.ToString("X8") + "><S1>";
                chartOffset += chartSize;
                return chartEntry;
            }

        }

        public static void Print(string text)
        {
            osd.Update(text);
        }

        static RivaTuner()
        {
            if (!IsRivaRunning())
            {
                RunRiva();
            } else
            {
                osd = new OSD("customRTSS");
            }
        }

        public static bool IsRivaRunning()
        {
            Process[] pname = Process.GetProcessesByName("RTSS");
            if (pname.Length == 0)
                return false;
            else
                return true;
        }

        public static bool VerifyRiva()
        {
           return File.Exists(rtss_exe);
        }

        public static void RunRiva()
        {
            FileInfo f = new FileInfo(rtss_exe);
            if (VerifyRiva())
            {
                try
                {
                    RtssInstance = Process.Start(f.FullName);
                    Thread.Sleep(2000);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static void KillRtss()
        {
            if (RtssInstance == null) return;
            try
            {
                RtssInstance.Kill();
                Process[] proc = Process.GetProcessesByName("RTSSHooksLoader64");
                proc[0].Kill();
            }
            catch (Exception) { }
            
        }

        public static string TextFormat()
        {
            return "<C0=" + LabelColor + "><C1=" + ColorBad+ "><C2=" + ColorMid + "><C3=" + ColorGood + "><S0=47><S1=65><S2=55><A0=-15><A1=55>";
        }

        

        public static void BuildRivaOutput()
        {
            string output = "";
            if(App.meterState.TickRate == 0 && App.meterState.Game == "")
            {
                PrintData(output, true);
                return;
            }
            chartOffset = 0;
           
            PrintData(output, true);
        }

        public static void PrintData(string text,bool RunRivaFlag = false)
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
