using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace YAKD.Utils
{
    class MouseHook : IDisposable
    {
        private enum HookType
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEHOOKSTRUCT
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

        HookType _hookType = HookType.WH_MOUSE_LL;
        IntPtr _hookHandle = IntPtr.Zero;
        HookProc _hookFunction;

        private delegate int HookProc(int code, IntPtr wParam, ref MOUSEHOOKSTRUCT lParam);

        public delegate void HookEventHandler(object sender, HookEventArgs e);
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
            switch ((MouseMessages)wParam)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    KeyDown?.Invoke(this, new HookEventArgs("Mouse LB"));
                    break;
                case MouseMessages.WM_LBUTTONUP:
                    KeyUp?.Invoke(this, new HookEventArgs("Mouse LB"));
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    KeyDown?.Invoke(this, new HookEventArgs("Mouse RB"));
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    KeyUp?.Invoke(this, new HookEventArgs("Mouse RB"));
                    break;
                case MouseMessages.WM_MBUTTONDOWN:
                    KeyDown?.Invoke(this, new HookEventArgs("Mouse MB"));
                    break;
                case MouseMessages.WM_MBUTTONUP:
                    KeyUp?.Invoke(this, new HookEventArgs("Mouse MB"));
                    break;
                case MouseMessages.WM_XBUTTONDOWN when lParam.hwnd.Equals((IntPtr)0x00010000):
                    KeyDown?.Invoke(this, new HookEventArgs("Mouse XB1"));
                    break;
                case MouseMessages.WM_XBUTTONUP when lParam.hwnd.Equals((IntPtr)0x00010000):
                    KeyUp?.Invoke(this, new HookEventArgs("Mouse XB1"));
                    break;
                case MouseMessages.WM_XBUTTONDOWN when lParam.hwnd.Equals((IntPtr)0x00020000):
                    KeyDown?.Invoke(this, new HookEventArgs("Mouse XB2"));
                    break;
                case MouseMessages.WM_XBUTTONUP when lParam.hwnd.Equals((IntPtr)0x00020000):
                    KeyUp?.Invoke(this, new HookEventArgs("Mouse XB2"));
                    break;
                case MouseMessages.WM_MOUSEWHEEL:
                {
                    if (lParam.hwnd.Equals((IntPtr)(-7864320)))
                        KeyDown?.Invoke(this, new HookEventArgs("Wheel Down"));
                    else if (lParam.hwnd.Equals((IntPtr)0x00780000))
                        KeyDown?.Invoke(this, new HookEventArgs("Wheel Up"));

                    WheelStopped();
                    break;
                }
            }

            return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        public async void WheelStopped()
        {
            await Task.Delay(100);
            KeyUp?.Invoke(this, new HookEventArgs("Wheel Down"));
            KeyUp?.Invoke(this, new HookEventArgs("Wheel Up"));
        }

        private void Install()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                return;
            }

            var list = Assembly.GetExecutingAssembly().GetModules();
            System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

            _hookHandle = SetWindowsHookEx(_hookType, _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
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

        public class HookEventArgs : EventArgs
        {
            public string Key;

            public HookEventArgs(string keyCode)
            {
                Key = keyCode;
            }
        }
    }
}
