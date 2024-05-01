using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using YAKD.Helpers;
using YAKD.Hooks.Keyboard;
using YAKD.Hooks.Mouse;
using YAKD.Models;

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

        private readonly List<KeyModel> _keys;

        private IntPtr _windowHandle;

        private readonly KeyDisplayerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyDisplayerForm class
        /// </summary>
        /// <param name="settings">Displaying settings</param>
        /// <param name="keysSettings">Keys settings</param>
        public KeyDisplayerForm(KeyDisplayerSettings settings, KeysSettings keysSettings)
        {
            _keys = new List<KeyModel>();
            _settings = settings ?? new KeyDisplayerSettings();

            InitializeComponent();
            InitializeSettings(settings);

            _keyboardHook = new KeyboardHook(keysSettings);
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
            Background = new SolidColorBrush(_settings.BackgroundColor)
            {
                Opacity = _settings.DisplayOnKeyPressedOnly && _keys.Count == 0 ? 0 : _settings.BackgroundColorOpacity
            };

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

            ApplyClickThroughWindowSetting();

            InitializeMouseHook(settings.MouseEnabled);
        }

        #endregion

        #region Handlers

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_settings.FixWindow)
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
                _keys.Sort((a, b) => b.DisplayName.Length.CompareTo(a.DisplayName.Length));
                KeysTextBlock.Text = string.Join(" + ", _keys.Select(x => x.DisplayName).Distinct());

                if (_settings.DisplayOnKeyPressedOnly)
                {
                    if (Background.Opacity == 0 && _keys.Count != 0)
                    {
                        Background.Opacity = _settings.BackgroundColorOpacity;
                    }
                    else if (Background.Opacity != 0 && _keys.Count == 0)
                    {
                        Background.Opacity = 0;
                    }
                }
            }
        }

        private void AddKey(KeyModel key)
        {
            if (_keys.All(x => x.Name != key.Name))
            {
                _keys.Add(key);
                ShowKeys();
            }
        }

        private async void RemoveKeyAsync(KeyModel key)
        {
            _keys.RemoveAll(x => x.Name == key.Name);

            if (_settings.DisplayDelay != 0)
            {
                await Task.Delay(_settings.DisplayDelay);
            }

            ShowKeys();
        }

        private void ApplyClickThroughWindowSetting()
        {
            if (_windowHandle != default)
            {
                if (_settings.ClickThroughWindow)
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
