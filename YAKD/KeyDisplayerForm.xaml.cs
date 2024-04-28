﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using YAKD.Helpers;
using YAKD.Hooks.Keyboard;
using YAKD.Hooks.Mouse;
using YAKD.Properties;

namespace YAKD
{
    /// <summary>
    /// Key displayer window
    /// </summary>
    public partial class KeyDisplayerForm : Window
    {
        #region Fields

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly KeyboardHook _keyboardHook;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private MouseHook _mouseHook;

        // For demo keys only
        private bool _isKeyboardHookEnabled;

        private bool _isWindowShouldBeFixed;

        private int _displayDelay;

        private bool _clickThroughWindow;

        private readonly List<string> _keys;

        private IntPtr _windowHandle;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyDisplayerForm class
        /// </summary>
        /// <param name="settings">Displaying settings</param>
        public KeyDisplayerForm(KeyDisplayerSettings settings)
        {
            if (settings == null)
            {
                settings = new KeyDisplayerSettings();
            }

            InitializeComponent();
            InitializeSettings(settings);
            _keys = new List<string>();

            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyDown += KeyboardHook_KeyDown;
            _keyboardHook.KeyUp += KeyboardHook_KeyUp;
        }

        #endregion

        #region Methods

        public void InitializeSettings(KeyDisplayerSettings settings)
        {
            KeysTextBlock.FontFamily = settings.FontFamily;
            KeysTextBlock.FontSize = settings.FontSize;
            KeysTextBlock.Foreground = new SolidColorBrush(settings.Color);
            KeysTextBlock.HorizontalAlignment = settings.KeysAlignment;
            var solidColor = new SolidColorBrush(settings.BackgroundColor)
            {
                Opacity = settings.BackgroundColorOpacity
            };
            Background = solidColor;
            if (settings.DemoKeys != "")
            {
                KeysTextBlock.Text = settings.DemoKeys;
                _isKeyboardHookEnabled = false;
            }
            else
            {
                KeysTextBlock.Text = "";
                _isKeyboardHookEnabled = true;
            }

            if (settings.StartupPoint != null)
            {
                Top = settings.StartupPoint.Top;
                Left = settings.StartupPoint.Left;
            }

            Height = settings.Height;
            Width = settings.Width;
            ResizeMode = settings.CanResize ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;
            _isWindowShouldBeFixed = settings.FixWindow;
            _displayDelay = settings.DisplayDelay;
            _clickThroughWindow = settings.ClickThroughWindow;

            ApplyClickThroughWindowSetting();

            InitializeMouseHook(settings.MouseEnabled);
        }

        #endregion

        #region Handlers

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isWindowShouldBeFixed)
            {
                DragMove();
            }
        }

        #endregion

        #region Events

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
            ApplyClickThroughWindowSetting();
        }

        private void KeyboardHook_KeyDown(object sender, KeyboardHookEventArgs e) => AddKey(e.Key);

        private void KeyboardHook_KeyUp(object sender, KeyboardHookEventArgs e) => RemoveKeyAsync(e.Key);

        private void MouseHook_KeyDown(object sender, MouseHookEventArgs e) => AddKey(e.Key);

        private void MouseHook_KeyUp(object sender, MouseHookEventArgs e) => RemoveKeyAsync(e.Key);

        #endregion

        #region Helpers

        private void InitializeMouseHook(bool isMouseEnabled)
        {
            if (isMouseEnabled)
            {
                if (_mouseHook == null)
                {
                    _mouseHook = new MouseHook();
                    _mouseHook.KeyDown += MouseHook_KeyDown;
                    _mouseHook.KeyUp += MouseHook_KeyUp;
                }
            }
            else
            {
                if (_mouseHook != null)
                {
                    _mouseHook.Dispose();
                    _mouseHook = null;
                }
            }
        }

        private void ShowKeys()
        {
            if (_isKeyboardHookEnabled)
            {
                _keys.Sort((a, b) => b.Length.CompareTo(a.Length));
                KeysTextBlock.Text = string.Join(" + ", _keys);
            }
        }

        private void AddKey(string key)
        {
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                ShowKeys();
            }
        }

        private async void RemoveKeyAsync(string key)
        {
            _keys.RemoveAll(x => x == key);

            if (_displayDelay != 0)
            {
                await Task.Delay(_displayDelay);
            }

            ShowKeys();
        }

        private void ApplyClickThroughWindowSetting()
        {
            if (_windowHandle != default)
            {
                if (_clickThroughWindow)
                {
                    WindowsService.SetWindowTransparent(_windowHandle);
                }
                else
                {
                    WindowsService.SetWindowNotTransparent(_windowHandle);
                }
            }
        }

        #endregion
    }
}
