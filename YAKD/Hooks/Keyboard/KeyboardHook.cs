using System;
using System.Reflection;
using System.Runtime.InteropServices;
using YAKD.Enums;

namespace YAKD.Hooks.Keyboard
{
    internal class KeyboardHook : IDisposable
    {
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            private IntPtr extraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr instance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        private const HookType HookType = Enums.HookType.WH_KEYBOARD_LL;
        private IntPtr _hookHandle = IntPtr.Zero;
        private readonly HookProc _hookFunction;

        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        public delegate void HookEventHandler(object sender, KeyboardHookEventArgs e);

        public event HookEventHandler KeyDown;
        public event HookEventHandler KeyUp;

        public KeyboardHook()
        {
            _hookFunction = HookCallback;
            Install();
        }

        ~KeyboardHook()
        {
            Uninstall();
        }

        public void Dispose()
        {
            Uninstall();
        }

        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
            }

            if ((lParam.flags & 0x80) != 0)
            {
                KeyUp?.Invoke(this, new KeyboardHookEventArgs(lParam.vkCode));
            }

            if ((lParam.flags & 0x80) == 0)
            {
                KeyDown?.Invoke(this, new KeyboardHookEventArgs(lParam.vkCode));
            }

            return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        private void Install()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                return;
            }

            var modules = Assembly.GetExecutingAssembly().GetModules();
            System.Diagnostics.Debug.Assert(modules != null && modules.Length > 0);

            _hookHandle = SetWindowsHookEx(HookType, _hookFunction, Marshal.GetHINSTANCE(modules[0]), 0);
        }

        private void Uninstall()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;

                KeyDown = KeyUp = null;
            }
        }
    }
}
