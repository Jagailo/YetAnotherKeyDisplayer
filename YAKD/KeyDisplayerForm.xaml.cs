using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    /// Key displayer window.
    /// </summary>
    public partial class KeyDisplayerForm : Window
    {
        #region Fields

        private readonly KeyboardHook _keyboardHook;

        private MouseHook _mouseHook;

        // For demo keys only
        private bool _isKeyboardHookEnabled;

        private readonly List<KeyModel> _keys;

        private readonly Dictionary<string, CancellationTokenSource> _pendingRemovals;

        private IntPtr _windowHandle;

        private readonly KeyDisplayerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyDisplayerForm class.
        /// </summary>
        /// <param name="settings">Displaying settings.</param>
        /// <param name="keysSettings">Keys settings.</param>
        public KeyDisplayerForm(KeyDisplayerSettings settings, KeysSettings keysSettings)
        {
            _keys = new List<KeyModel>();
            _pendingRemovals = new Dictionary<string, CancellationTokenSource>();
            _settings = settings ?? new KeyDisplayerSettings();

            InitializeComponent();
            ShowInTaskbar = false;

            InitializeSettings(settings);

            _keyboardHook = new KeyboardHook(keysSettings);
            _keyboardHook.KeyDown += KeyboardHook_KeyDown;
            _keyboardHook.KeyUp += KeyboardHook_KeyUp;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
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

        protected override void OnClosed(EventArgs e)
        {
            foreach (var keyName in _pendingRemovals.Keys.ToList())
            {
                CancelPendingRemoval(keyName);
            }

            _keyboardHook.Dispose();
            _mouseHook?.Dispose();
            _mouseHook = null;
            base.OnClosed(e);
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
            var isChanged = false;

            foreach (var releasedKeyName in _pendingRemovals.Keys.ToList())
            {
                CancelPendingRemoval(releasedKeyName);

                if (releasedKeyName != key.Name)
                {
                    isChanged |= _keys.RemoveAll(x => x.Name == releasedKeyName) > 0;
                }
            }

            if (_keys.All(x => x.Name != key.Name))
            {
                _keys.Add(key);
                isChanged = true;
            }

            if (isChanged)
            {
                ShowKeys();
            }
        }

        private async void RemoveKeyAsync(KeyModel key)
        {
            try
            {
                if (_settings.DisplayDelay != 0)
                {
                    CancelPendingRemoval(key.Name);

                    using (var cancellation = new CancellationTokenSource())
                    {
                        _pendingRemovals[key.Name] = cancellation;

                        try
                        {
                            await Task.Delay(_settings.DisplayDelay, cancellation.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            return;
                        }

                        if (cancellation.IsCancellationRequested)
                        {
                            return;
                        }

                        _pendingRemovals.Remove(key.Name);
                    }
                }

                _keys.RemoveAll(x => x.Name == key.Name);
                ShowKeys();
            }
            catch
            {
                // Ignored
            }
        }

        private void CancelPendingRemoval(string keyName)
        {
            if (_pendingRemovals.TryGetValue(keyName, out var cancellation))
            {
                _pendingRemovals.Remove(keyName);
                cancellation.Cancel();
            }
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
