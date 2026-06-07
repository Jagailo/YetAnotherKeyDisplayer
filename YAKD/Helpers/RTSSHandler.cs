using Microsoft.Win32;
using RTSSSharedMemoryNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;

namespace YAKD.Helpers
{
    /// <summary>
    /// Provides helper methods for working with RTSS.
    /// </summary>
    public static class RtssHandler
    {
        #region Fields

        private static Process _rtssInstance;

        private static bool _startedByYakd;

        private static OSD _osd;

        private const string OsdEntryName = "YAKDOSD";

        private const string RtssSharedMemoryName = "RTSSSharedMemoryV2";

        private static readonly string[] RtssHelperProcessNames =
        {
            "RTSSHooksLoader",
            "RTSSHooksLoader64",
            "EncoderServer",
            "EncoderServer64"
        };

        private const uint FileMapAllAccess = 0x000F001F;

        private const int SharedMemoryWaitTimeout = 6000;
        private const int SharedMemoryCheckInterval = 250;

        #endregion

        #region Properties

        /// <summary>
        /// Path to the RTSS executable.
        /// </summary>
        public static string RtssPath { get; set; }

        /// <summary>
        /// Gets a value indicating whether RTSS is currently running.
        /// </summary>
        public static bool IsRtssRunning => Process.GetProcessesByName("RTSS").Length != 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RTSSHandler class.
        /// </summary>
        static RtssHandler()
        {
            RtssPath = ResolveRtssPath();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send text to RTSS.
        /// </summary>
        /// <param name="text">Text.</param>
        public static void Print(string text)
        {
            if (!IsRtssRunning)
            {
                return;
            }

            if (_osd == null)
            {
                RunOsd();
            }

            if (_osd != null && !_osd.Update(SanitizeTextForRtss(text)))
            {
                DisposeOsd();
            }
        }

        /// <summary>
        /// Launches RTSS.
        /// </summary>
        public static void RunRtss()
        {
            if (IsRtssRunning)
            {
                if (!IsOurInstanceAlive())
                {
                    _rtssInstance = null;
                    _startedByYakd = false;
                }

                RunOsd();

                return;
            }

            if (!File.Exists(RtssPath))
            {
                return;
            }

            DisposeOsd();
            _rtssInstance = null;
            _startedByYakd = false;

            try
            {
                _rtssInstance = Process.Start(RtssPath);
                _startedByYakd = true;

                if (WaitForSharedMemory())
                {
                    RunOsd();
                }
            }
            catch (Exception exception)
            {
                Logger.Write(exception);

                MessageBox.Show(exception.Message, "Could not start RTSS", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Releases the YAKD OSD slot without touching the RTSS process.
        /// Use this when switching display modes: RTSS keeps running, only YAKD's overlay disappears.
        /// </summary>
        public static void ReleaseOsd()
        {
            DisposeOsd();
        }

        /// <summary>
        /// Releases the YAKD OSD slot and closes RTSS, but only if YAKD launched it itself.
        /// An RTSS instance started by the user is left running so YAKD never shuts down something it did not start.
        /// </summary>
        public static void Shutdown()
        {
            DisposeOsd();

            var instance = _rtssInstance;
            var startedByYakd = _startedByYakd;

            _rtssInstance = null;
            _startedByYakd = false;

            if (!startedByYakd)
            {
                return;
            }

            try
            {
                if (instance != null && !instance.HasExited)
                {
                    instance.Kill();
                }
            }
            catch (Exception exception)
            {
                Logger.Write(exception);
            }
            finally
            {
                instance?.Dispose();
            }

            KillHelperProcesses();
        }

        #endregion

        #region Helpers

        private static bool IsOurInstanceAlive()
        {
            try
            {
                return _rtssInstance != null && !_rtssInstance.HasExited;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void KillHelperProcesses()
        {
            foreach (var name in RtssHelperProcessNames)
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName(name))
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }
                        }
                        catch (Exception)
                        {
                            // Ignored
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
        }

        private static void RunOsd()
        {
            if (_osd != null)
            {
                return;
            }

            if (!IsSharedMemoryAvailable())
            {
                return;
            }

            try
            {
                _osd = new OSD(OsdEntryName);
            }
            catch (Exception exception)
            {
                Logger.Write(exception);

                MessageBox.Show(exception.Message, "Could not start OSD", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void DisposeOsd()
        {
            if (_osd == null)
            {
                return;
            }

            try
            {
                _osd.Dispose();
            }
            catch (Exception exception)
            {
                Logger.Write(exception);
            }
            finally
            {
                _osd = null;
            }
        }

        private static bool WaitForSharedMemory()
        {
            for (var waited = 0; waited < SharedMemoryWaitTimeout; waited += SharedMemoryCheckInterval)
            {
                if (IsRtssRunning && IsSharedMemoryAvailable())
                {
                    return true;
                }

                Thread.Sleep(SharedMemoryCheckInterval);
            }

            return IsRtssRunning && IsSharedMemoryAvailable();
        }

        private static string ResolveRtssPath()
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (var rtssKey = baseKey.OpenSubKey(@"SOFTWARE\Unwinder\RTSS"))
                    {
                        if (rtssKey?.GetValue("InstallPath") is string installPath && !string.IsNullOrWhiteSpace(installPath) && File.Exists(installPath))
                        {
                            return installPath;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Write(exception);
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "RivaTuner Statistics Server", "RTSS.exe");
        }

        private static bool IsSharedMemoryAvailable()
        {
            var handle = OpenFileMapping(FileMapAllAccess, false, RtssSharedMemoryName);
            if (handle == IntPtr.Zero)
            {
                return false;
            }

            CloseHandle(handle);

            return true;
        }

        private static string SanitizeTextForRtss(string text)
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

        #region Native methods

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion
    }
}
