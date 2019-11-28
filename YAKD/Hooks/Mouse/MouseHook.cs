using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YAKD.Enums;

namespace YAKD.Hooks.Mouse
{
    internal class MouseHook : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEHOOKSTRUCT
        {
            public Point pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr instance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hook, int code, IntPtr wParam, ref MOUSEHOOKSTRUCT lParam);

        private const HookType HookType = Enums.HookType.WH_MOUSE_LL;
        private IntPtr hookHandle = IntPtr.Zero;
        private readonly HookProc hookFunction;

        private delegate int HookProc(int code, IntPtr wParam, ref MOUSEHOOKSTRUCT lParam);

        public delegate void HookEventHandler(object sender, MouseHookEventArgs e);

        public event HookEventHandler KeyDown;
        public event HookEventHandler KeyUp;

        public MouseHook()
        {
            hookFunction = HookCallback;
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

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDOWN = 0x0207
        }

        private int HookCallback(int code, IntPtr wParam, ref MOUSEHOOKSTRUCT lParam)
        {
            switch ((MouseMessages) wParam)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    KeyDown?.Invoke(this, new MouseHookEventArgs("Mouse LB"));
                    break;
                case MouseMessages.WM_LBUTTONUP:
                    KeyUp?.Invoke(this, new MouseHookEventArgs("Mouse LB"));
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    KeyDown?.Invoke(this, new MouseHookEventArgs("Mouse RB"));
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    KeyUp?.Invoke(this, new MouseHookEventArgs("Mouse RB"));
                    break;
                case MouseMessages.WM_MBUTTONDOWN:
                    KeyDown?.Invoke(this, new MouseHookEventArgs("Mouse MB"));
                    break;
                case MouseMessages.WM_MBUTTONUP:
                    KeyUp?.Invoke(this, new MouseHookEventArgs("Mouse MB"));
                    break;
                case MouseMessages.WM_XBUTTONDOWN when lParam.hwnd.Equals((IntPtr) 0x00010000):
                    KeyDown?.Invoke(this, new MouseHookEventArgs("Mouse XB1"));
                    break;
                case MouseMessages.WM_XBUTTONUP when lParam.hwnd.Equals((IntPtr) 0x00010000):
                    KeyUp?.Invoke(this, new MouseHookEventArgs("Mouse XB1"));
                    break;
                case MouseMessages.WM_XBUTTONDOWN when lParam.hwnd.Equals((IntPtr) 0x00020000):
                    KeyDown?.Invoke(this, new MouseHookEventArgs("Mouse XB2"));
                    break;
                case MouseMessages.WM_XBUTTONUP when lParam.hwnd.Equals((IntPtr) 0x00020000):
                    KeyUp?.Invoke(this, new MouseHookEventArgs("Mouse XB2"));
                    break;
                case MouseMessages.WM_MOUSEWHEEL:
                {
                    if (lParam.hwnd.Equals((IntPtr) (-7864320)))
                    {
                        KeyDown?.Invoke(this, new MouseHookEventArgs("Wheel Down"));
                        KeyUp?.Invoke(this, new MouseHookEventArgs("Wheel Up"));
                    }
                    else if (lParam.hwnd.Equals((IntPtr) 0x00780000))
                    {
                        KeyDown?.Invoke(this, new MouseHookEventArgs("Wheel Up"));
                        KeyUp?.Invoke(this, new MouseHookEventArgs("Wheel Down"));
                    }

                    WheelStopped();
                    break;
                }
            }

            return CallNextHookEx(hookHandle, code, wParam, ref lParam);
        }

        private async void WheelStopped()
        {
            await Task.Delay(100);
            KeyUp?.Invoke(this, new MouseHookEventArgs("Wheel Down"));
            KeyUp?.Invoke(this, new MouseHookEventArgs("Wheel Up"));
        }

        private void Install()
        {
            if (hookHandle != IntPtr.Zero)
            {
                return;
            }

            var list = Assembly.GetExecutingAssembly().GetModules();
            System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

            hookHandle = SetWindowsHookEx(HookType, hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
        }

        private void Uninstall()
        {
            if (hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookHandle);
                hookHandle = IntPtr.Zero;

                KeyDown = KeyUp = null;
            }
        }
    }
}
