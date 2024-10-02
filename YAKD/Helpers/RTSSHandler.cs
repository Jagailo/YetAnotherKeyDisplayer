using RTSSSharedMemoryNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;

namespace YAKD.Helpers
{
    /// <summary>
    /// A class that works with RTSS
    /// </summary>
    public static class RTSSHandler
    {
        #region Fields

        private static Process _rtssInstance;

        private static OSD _osd;

        #endregion

        #region Properties

        /// <summary>
        /// Path to RTSS
        /// </summary>
        public static string RTSSPath { get; set; }

        /// <summary>
        /// Returns true if RTSS is running
        /// </summary>
        public static bool IsRTSSRunning => Process.GetProcessesByName("RTSS").Length != 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RTSSHandler class
        /// </summary>
        static RTSSHandler()
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Unwinder\RTSS\InstallDir
            RTSSPath = @"C:\Program Files (x86)\RivaTuner Statistics Server\RTSS.exe";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send text to RTSS
        /// </summary>
        /// <param name="text">Text</param>
        public static void Print(string text)
        {
            if (IsRTSSRunning)
            {
                _osd?.Update(text);
            }
        }

        /// <summary>
        /// Launches RTSS
        /// </summary>
        public static void RunRTSS()
        {
            if (IsRTSSRunning)
            {
                RunOSD();
            }
            else if (File.Exists(RTSSPath))
            {
                KillRTSS();

                try
                {
                    _rtssInstance = Process.Start(RTSSPath);
                    WaitForRTSSStartup();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Could not start the RTSS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static void WaitForRTSSStartup()
        {
            const int maxWaitTime = 5000;
            const int checkInterval = 500;

            for (var waited = 0; waited < maxWaitTime; waited += checkInterval)
            {
                if (IsRTSSRunning)
                {
                    return;
                }

                Thread.Sleep(checkInterval);
            }
        }

        /// <summary>
        /// Launches OSD
        /// </summary>
        private static void RunOSD()
        {
            if (_osd == null)
            {
                try
                {
                    _osd = new OSD("YAKDOSD");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Could not start the OSD", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Closes RTSS
        /// </summary>
        public static void KillRTSS()
        {
            if (_rtssInstance != null)
            {
                try
                {
                    _rtssInstance.Kill();
                    _rtssInstance.Dispose();
                }
                catch (Exception)
                {
                    // Ignored
                }
                finally
                {
                    _rtssInstance = null;
                }

                try
                {
                    var hooksLoader = Process.GetProcessesByName("RTSSHooksLoader64").FirstOrDefault();
                    hooksLoader?.Kill();
                    hooksLoader?.Dispose();
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
        }

        #endregion
    }
}
