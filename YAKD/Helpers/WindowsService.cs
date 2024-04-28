using System;
using System.Runtime.InteropServices;

namespace YAKD.Helpers
{
    public static class WindowsService
    {
        #region Fields

        private const int WS_EX_TRANSPARENT = 0x00000020;

        private const int GWL_EXSTYLE = -20;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the transparency bit for the specified window
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        public static void SetWindowTransparent(IntPtr hwnd)
        {
            var currentWindowStyles = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, currentWindowStyles | WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Removes the transparency bit for the specified window
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        public static void SetWindowNotTransparent(IntPtr hwnd)
        {
            var currentWindowStyles = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, currentWindowStyles & ~WS_EX_TRANSPARENT);
        }

        #endregion

        #region Helpers

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        #endregion
    }
}
