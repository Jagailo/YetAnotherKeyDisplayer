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
    /// Provides helper methods for working with RTSS.
    /// </summary>
    public static class RTSSHandler
    {
        #region Fields

        private static Process _rtssInstance;

        private static OSD _osd;

        #endregion

        #region Properties

        /// <summary>
        /// Path to the RTSS executable.
        /// </summary>
        public static string RTSSPath { get; set; }

        /// <summary>
        /// Gets a value indicating whether RTSS is currently running.
        /// </summary>
        public static bool IsRTSSRunning => Process.GetProcessesByName("RTSS").Length != 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RTSSHandler class.
        /// </summary>
        static RTSSHandler()
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Unwinder\RTSS\InstallDir
            RTSSPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "RivaTuner Statistics Server", "RTSS.exe");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send text to RTSS.
        /// </summary>
        /// <param name="text">Text.</param>
        public static void Print(string text)
        {
            if (IsRTSSRunning)
            {
                if (_osd == null)
                {
                    RunOSD();
                }

                _osd?.Update(SanitizeTextForRTSS(text));
            }
        }

        /// <summary>
        /// Launches RTSS.
        /// </summary>
        public static void RunRTSS()
        {
            if (IsRTSSRunning)
            {
                RunOSD();
                return;
            }

            if (File.Exists(RTSSPath))
            {
                KillRTSS();

                try
                {
                    _rtssInstance = Process.Start(RTSSPath);
                    WaitForRTSSStartup();

                    if (IsRTSSRunning)
                    {
                        RunOSD();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Could not start the RTSS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Closes RTSS.
        /// </summary>
        public static void KillRTSS()
        {
            if (_osd != null)
            {
                try
                {
                    _osd.Dispose();
                }
                catch (Exception)
                {
                    // Ignored
                }
                finally
                {
                    _osd = null;
                }
            }

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

        #region Helpers

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

        private static string SanitizeTextForRTSS(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var sanitizedText = text;

            sanitizedText = sanitizedText.Replace("\u2191", "Up");
            sanitizedText = sanitizedText.Replace("\u2192", "Right");
            sanitizedText = sanitizedText.Replace("\u2193", "Down");
            sanitizedText = sanitizedText.Replace("\u2190", "Left");

            return sanitizedText;
        }

        #endregion
    }
}
