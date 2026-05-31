using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using YAKD.Enums;
using YAKD.Models;

namespace YAKD.Hooks.Keyboard
{
    internal class KeyboardHook : IDisposable
    {
        private static readonly HashSet<uint> IgnoredVkCodes = new HashSet<uint>
        {
            0x15, // VK_KANA / VK_HANGUL
            0x16, // VK_IME_ON
            0x17, // VK_JUNJA
            0x18, // VK_FINAL
            0x19, // VK_KANJI / VK_HANJA
            0x1A, // VK_IME_OFF
            0x1C, // VK_CONVERT
            0x1D, // VK_NONCONVERT
            0x1E, // VK_ACCEPT
            0x1F, // VK_MODECHANGE
            0xE5 // VK_PROCESSKEY
        };

        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            private IntPtr extraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr instance, int threadId);

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

        private readonly KeysSettings _keysSettings;

        public KeyboardHook(KeysSettings settings)
        {
            _keysSettings = settings;
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

            if (IgnoredVkCodes.Contains(lParam.vkCode))
            {
                return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
            }

            if ((lParam.flags & 0x80) != 0)
            {
                KeyUp?.Invoke(this, new KeyboardHookEventArgs(lParam.vkCode, _keysSettings));
            }

            if ((lParam.flags & 0x80) == 0)
            {
                KeyDown?.Invoke(this, new KeyboardHookEventArgs(lParam.vkCode, _keysSettings));
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
