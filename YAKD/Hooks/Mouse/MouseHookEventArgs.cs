﻿using System;

namespace YAKD.Hooks.Mouse
{
    internal class MouseHookEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Key
        /// </summary>
        public readonly string Key;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MouseHookEventArgs class
        /// </summary>
        /// <param name="keyCode">Mouse key code from WinAPI hook</param>
        public MouseHookEventArgs(string keyCode)
        {
            Key = keyCode;
        }

        #endregion
    }
}
