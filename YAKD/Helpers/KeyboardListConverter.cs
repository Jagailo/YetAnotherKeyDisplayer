﻿using System.Collections.Generic;

namespace YAKD.Helpers
{
    /// <summary>
    /// Converts key names to more understandable and simple ones
    /// </summary>
    public static class KeyboardListConverter
    {
        #region Fields

        /// <summary>
        /// List of beautiful keys
        /// </summary>
        private static readonly Dictionary<string, string> Keys;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of KeyboardListConverter class with list of keys
        /// </summary>
        static KeyboardListConverter()
        {
            Keys = new Dictionary<string, string>
            {
                { "Add", "Numpad +" },
                { "Back", "Backspace" },
                { "Capital", "Caps Lock" },
                { "D0", "0" },
                { "D1", "1" },
                { "D2", "2" },
                { "D3", "3" },
                { "D4", "4" },
                { "D5", "5" },
                { "D6", "6" },
                { "D7", "7" },
                { "D8", "8" },
                { "D9", "9" },
                { "Decimal", "Numpad ." },
                { "Divide", "Numpad /" },
                { "Escape", "Esc" },
                { "LControlKey", "L Ctrl" },
                { "LMenu", "L Alt" },
                { "LShiftKey", "L Shift" },
                { "LWin", "L Win" },
                { "Multiply", "Numpad *" },
                { "Next", "Page Down" },
                { "NumLock", "Num Lock" },
                { "NumPad0", "Numpad 0" },
                { "NumPad1", "Numpad 1" },
                { "NumPad2", "Numpad 2" },
                { "NumPad3", "Numpad 3" },
                { "NumPad4", "Numpad 4" },
                { "NumPad5", "Numpad 5" },
                { "NumPad6", "Numpad 6" },
                { "NumPad7", "Numpad 7" },
                { "NumPad8", "Numpad 8" },
                { "NumPad9", "Numpad 9" },
                { "Oem1", ":" },
                { "Oem5", "\\" },
                { "Oem6", "}" },
                { "Oem7", "\"" },
                { "Oemcomma", "<" },
                { "OemMinus", "-" },
                { "OemOpenBrackets", "{" },
                { "OemPeriod", ">" },
                { "Oemplus", "+" },
                { "OemQuestion", "?" },
                { "Oemtilde", "~" },
                { "PageUp", "Page Up" },
                { "PrintScreen", "PtrScr" },
                { "RControlKey", "R Ctrl" },
                { "Return", "Enter" },
                { "RMenu", "R Alt" },
                { "RShiftKey", "R Shift" },
                { "RWin", "R Win" },
                { "Scroll", "Scroll Lock" },
                { "Subtract", "Numpad -" }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns key name
        /// </summary>
        /// <param name="winApiKeyName">Name of key from WinApi hook</param>
        /// <returns>Key name</returns>
        public static string GetKeyName(string winApiKeyName)
        {
            return Keys.TryGetValue(winApiKeyName, out var keyName) ? keyName : winApiKeyName;
        }

        #endregion
    }
}
