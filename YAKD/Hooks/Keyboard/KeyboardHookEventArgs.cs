using System;
using System.Windows.Forms;
using YAKD.Helpers;

namespace YAKD.Hooks.Keyboard
{
    internal class KeyboardHookEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Key
        /// </summary>
        public readonly string Key;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyboardHookEventArgs class
        /// </summary>
        /// <param name="keyCode">Key code from WinAPI hook</param>
        public KeyboardHookEventArgs(uint keyCode)
        {
            Key = KeyboardListConverter.GetKeyName(((Keys)keyCode).ToString());
        }

        #endregion
    }
}
