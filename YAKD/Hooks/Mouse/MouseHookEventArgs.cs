using System;

namespace YAKD.Hooks.Mouse
{
    internal class MouseHookEventArgs : EventArgs
    {
        public readonly string Key;

        public MouseHookEventArgs(string keyCode)
        {
            Key = keyCode;
        }
    }
}
