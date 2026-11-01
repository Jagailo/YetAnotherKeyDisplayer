using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YAKD.Enums;
using YAKD.Helpers;

namespace YAKD.Hooks.Mouse
{
    internal class MouseHook : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            private readonly int X;
            private readonly int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            private readonly Point pt;
            public readonly uint mouseData;
            private readonly uint flags;
            private readonly uint time;
            private readonly IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr instance, int threadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hook, int code, IntPtr wParam, ref MSLLHOOKSTRUCT lParam);

        private const HookType HookType = Enums.HookType.WH_MOUSE_LL;
        private IntPtr _hookHandle = IntPtr.Zero;
        private readonly HookProc _hookFunction;

        private delegate int HookProc(int code, IntPtr wParam, ref MSLLHOOKSTRUCT lParam);

        public delegate void HookEventHandler(object sender, MouseHookEventArgs e);

        public event HookEventHandler KeyDown;

        public event HookEventHandler KeyUp;

        public MouseHook()
        {
            _hookFunction = HookCallback;
            Install();
        }

        ~MouseHook()
        {
            Uninstall();
        }

        public void Dispose()
        {
            Uninstall();
        }

        private int HookCallback(int code, IntPtr wParam, ref MSLLHOOKSTRUCT lParam)
        {
            var mouseParam = (MouseMessage)wParam;

            // The high word of mouseData holds the X button number or the signed wheel delta, which is not always a multiple of WHEEL_DELTA.
            var highWord = (short)(lParam.mouseData >> 16);
            switch (mouseParam)
            {
                case MouseMessage.WM_LBUTTONDOWN:
                case MouseMessage.WM_RBUTTONDOWN:
                case MouseMessage.WM_MBUTTONDOWN:
                {
                    KeyDown?.Invoke(this, new MouseHookEventArgs(MouseButtonsConverter.GetButtonName(mouseParam)));

                    break;
                }
                case MouseMessage.WM_LBUTTONUP:
                case MouseMessage.WM_RBUTTONUP:
                case MouseMessage.WM_MBUTTONUP:
                {
                    KeyUp?.Invoke(this, new MouseHookEventArgs(MouseButtonsConverter.GetButtonName(mouseParam)));

                    break;
                }
                case MouseMessage.WM_XBUTTONDOWN when highWord == 1 || highWord == 2:
                {
                    KeyDown?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} {highWord}"));

                    break;
                }
                case MouseMessage.WM_XBUTTONUP when highWord == 1 || highWord == 2:
                {
                    KeyUp?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} {highWord}"));

                    break;
                }
                case MouseMessage.WM_MOUSEWHEEL when highWord < 0:
                {
                    KeyUp?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} Up"));
                    KeyDown?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} Down"));
                    StopMouseWheelAsync("Down");

                    break;
                }
                case MouseMessage.WM_MOUSEWHEEL when highWord > 0:
                {
                    KeyUp?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} Down"));
                    KeyDown?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(mouseParam)} Up"));
                    StopMouseWheelAsync("Up");

                    break;
                }
            }

            return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        private async void StopMouseWheelAsync(string wheelDirection)
        {
            await Task.Delay(300);
            KeyUp?.Invoke(this, new MouseHookEventArgs($"{MouseButtonsConverter.GetButtonName(MouseMessage.WM_MOUSEWHEEL)} {wheelDirection}"));
        }

        private void Install()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                return;
            }

            var list = Assembly.GetExecutingAssembly().GetModules();
            Debug.Assert(list != null && list.Length > 0);

            _hookHandle = SetWindowsHookEx(HookType, _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
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
