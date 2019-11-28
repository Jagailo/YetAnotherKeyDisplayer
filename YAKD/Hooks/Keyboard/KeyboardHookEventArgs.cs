using System;
using System.Windows.Forms;
using YAKD.Helpers;

namespace YAKD.Hooks.Keyboard
{
    internal class KeyboardHookEventArgs : EventArgs
    {
        public readonly string Key;

        public KeyboardHookEventArgs(uint keyCode)
        {
            Key = KeyboardListConverter.GetKeyName(((Keys)keyCode).ToString());
        }
    }
}
