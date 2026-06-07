using System;
using System.Diagnostics;

namespace YAKD.Helpers
{
    /// <summary>
    /// Provides helper methods for writing debug log messages.
    /// </summary>
    public static class Logger
    {
        #region Methods

        /// <summary>
        /// Writes the specified exception to the debug output with a timestamp.
        /// </summary>
        /// <param name="exception">The exception to write.</param>
        public static void Write(Exception exception)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {exception}");
        }

        #endregion
    }
}
