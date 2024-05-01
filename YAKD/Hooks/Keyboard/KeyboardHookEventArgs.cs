using System;
using System.Windows.Forms;
using YAKD.Helpers;
using YAKD.Models;

namespace YAKD.Hooks.Keyboard
{
    internal class KeyboardHookEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Key
        /// </summary>
        public readonly KeyModel Key;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyboardHookEventArgs class
        /// </summary>
        /// <param name="keyCode">Key code from WinAPI hook</param>
        /// <param name="settings">Settings</param>
        public KeyboardHookEventArgs(uint keyCode, KeysSettings settings)
        {
            Key = new KeyModel(KeyboardListConverter.GetKeyName(((Keys)keyCode).ToString()), settings);
        }

        #endregion
    }
}
