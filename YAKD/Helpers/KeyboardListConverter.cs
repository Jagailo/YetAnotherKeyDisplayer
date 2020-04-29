using System.Collections.Generic;

namespace YAKD.Helpers
{
    /// <summary>
    /// Converts key names to more understandable and simple ones
    /// </summary>
    public static class KeyboardListConverter
    {
        #region Fields

        /// <summary>
        /// List of beautiful named keys
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
                { "add", "Numpad +" },
                { "back", "Backspace" },
                { "capital", "Caps Lock" },
                { "d0", "0" },
                { "d1", "1" },
                { "d2", "2" },
                { "d3", "3" },
                { "d4", "4" },
                { "d5", "5" },
                { "d6", "6" },
                { "d7", "7" },
                { "d8", "8" },
                { "d9", "9" },
                { "decimal", "Numpad ." },
                { "divide", "Numpad /" },
                { "escape", "Esc" },
                { "lcontrolkey", "L Ctrl" },
                { "lmenu", "L Alt" },
                { "lshiftkey", "L Shift" },
                { "lwin", "L Win" },
                { "multiply", "Numpad *" },
                { "next", "Page Down" },
                { "numlock", "Num Lock" },
                { "numpad0", "Numpad 0" },
                { "numpad1", "Numpad 1" },
                { "numpad2", "Numpad 2" },
                { "numpad3", "Numpad 3" },
                { "numpad4", "Numpad 4" },
                { "numpad5", "Numpad 5" },
                { "numpad6", "Numpad 6" },
                { "numpad7", "Numpad 7" },
                { "numpad8", "Numpad 8" },
                { "numpad9", "Numpad 9" },
                { "oem1", ":" },
                { "oem5", "\\" },
                { "oem6", "]" },
                { "oem7", "\"" },
                { "oembackslash", "/" },
                { "oemcomma", "<" },
                { "oemminus", "-" },
                { "oemopenbrackets", "[" },
                { "oemperiod", ">" },
                { "oemplus", "+" },
                { "oemquestion", "?" },
                { "oemtilde", "~" },
                { "pageup", "Page Up" },
                { "printscreen", "PtrScr" },
                { "rcontrolkey", "R Ctrl" },
                { "return", "Enter" },
                { "rmenu", "R Alt" },
                { "rshiftkey", "R Shift" },
                { "rwin", "R Win" },
                { "scroll", "Scroll Lock" },
                { "subtract", "Numpad -" }
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
            return Keys.TryGetValue(winApiKeyName.ToLower(), out var keyName) ? keyName : winApiKeyName;
        }

        #endregion
    }
}
