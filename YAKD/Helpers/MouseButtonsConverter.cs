using System.Collections.Generic;
using YAKD.Enums;

namespace YAKD.Helpers
{
    /// <summary>
    /// Converts button names to more understandable and simple ones
    /// </summary>
    public static class MouseButtonsConverter
    {
        #region Fields

        /// <summary>
        /// List of beautiful named buttons
        /// </summary>
        private static readonly Dictionary<MouseMessage, string> Buttons;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MouseButtonsConverter class
        /// </summary>
        static MouseButtonsConverter()
        {
            Buttons = new Dictionary<MouseMessage, string>
            {
                { MouseMessage.WM_LBUTTONUP, "LMB" },
                { MouseMessage.WM_LBUTTONDOWN, "LMB" },
                { MouseMessage.WM_LBUTTONDBLCLK, "LMB" },
                { MouseMessage.WM_RBUTTONUP, "RMB" },
                { MouseMessage.WM_RBUTTONDOWN, "RMB" },
                { MouseMessage.WM_RBUTTONDBLCLK, "RMB" },
                { MouseMessage.WM_MBUTTONUP, "MMB" },
                { MouseMessage.WM_MBUTTONDOWN, "MMB" },
                { MouseMessage.WM_MBUTTONDBLCLK, "MMB" },
                { MouseMessage.WM_XBUTTONUP, "XMB" },
                { MouseMessage.WM_XBUTTONDOWN, "XMB" },
                { MouseMessage.WM_XBUTTONDBLCLK, "XMB" }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns button name
        /// </summary>
        /// <param name="mouseMessages">Name of button from WinApi hook</param>
        /// <returns>Button name</returns>
        public static string GetButtonName(MouseMessage mouseMessages)
        {
            return Buttons.TryGetValue(mouseMessages, out var buttonName) ? buttonName : mouseMessages.ToString();
        }

        #endregion
    }
}
