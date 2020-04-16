using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YAKD.Helpers;
using YAKD.Hooks.Keyboard;
using YAKD.Hooks.Mouse;
using YAKD.Models;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace YAKD
{
    /// <summary>
    /// Main window
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private KeyDisplayerForm _keyDisplayerForm;

        private KeyDisplayerSettings _settings;

        private readonly bool _isFirstRun;

        private bool _isSliderEnabled, _isRtssEnabled, _isMouseEnabled;

        private KeyboardHook _keyboardHook;

        private MouseHook _mouseHook;

        private readonly List<string> _keys;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MainWindow class
        /// </summary>
        public MainWindow()
        {
            _settings = new KeyDisplayerSettings();
            _isSliderEnabled = true;
            _isRtssEnabled = false;
            _isMouseEnabled = false;
            _keys = new List<string>();
            InitializeSettingsFromFile(Properties.Settings.Default);
            _isFirstRun = true;
            InitializeComponent();
            _isFirstRun = false;
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeMainWindow(_settings);
            InitializeKeyDisplayerForm(_settings);
            _keyDisplayerForm.Show();
            Focus();
        }

        #endregion

        #region Handlers

        #region Key Displayer Mode

        private void RTSSRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableControls(false);
            RTSSRadioButton.IsChecked = true;
            e.Handled = true;
            _isRtssEnabled = true;
            RTSSHandler.RunRTSS();

            if (RTSSHandler.IsRTSSRunning())
            {
                InitializeKeyboardHook();
                if (_isMouseEnabled)
                {
                    EnableMouseHook();
                }
            }
            else
            {
                var rtssWindow = new RTSSWindow(RTSSHandler.RTSSPath);
                rtssWindow.ShowDialog();
                if (TransferModel.RTSSPath != null)
                {
                    RTSSHandler.RTSSPath = TransferModel.RTSSPath;
                }
                WindowRadioButton.IsChecked = true;
            }
        }

        private void WindowRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_isMouseEnabled)
            {
                DisableMouseHook();
                _settings.MouseEnabled = true;
            }
            else
            {
                _settings.MouseEnabled = false;
            }

            EnableControls(true);
            _isRtssEnabled = false;
            if (_keyboardHook != null)
            {
                _keyboardHook.Dispose();
                _keyboardHook = null;
            }
        }

        private void MouseHookCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_isRtssEnabled)
            {
                EnableMouseHook();
            }
            else
            {
                _isMouseEnabled = _settings.MouseEnabled = true;
                UpdateKeyDisplayerForm();
            }
        }

        private void MouseHookCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isRtssEnabled)
            {
                DisableMouseHook();
            }
            else
            {
                _isMouseEnabled = _settings.MouseEnabled = false;
                UpdateKeyDisplayerForm();
            }
        }

        #endregion

        #region Window

        private void BackgroundColorRectangle_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerWindow = new ColorPickerWindow(_settings.BackgroundColor);
            colorPickerWindow.ShowDialog();
            _settings.BackgroundColor = TransferModel.SelectedColor;
            BackgroundColorRectangle.Background = new SolidColorBrush(TransferModel.SelectedColor);
            UpdateKeyDisplayerForm();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OpacityTextBox != null && _isSliderEnabled)
            {
                OpacityTextBox.Text = OpacitySlider.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void OpacityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = OpacityTextBox.Text.Replace('.', ',');
            if (double.TryParse(value, out var number) && number >= 0.01 && number <= 1)
            {
                _settings.BackgroundColorOpacity = number;
                _isSliderEnabled = false;
                OpacitySlider.Value = number;
                _isSliderEnabled = true;
                UpdateKeyDisplayerForm();
            }
        }

        private void CheckBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void DemoKeysCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _settings.EnableDemoKeys(DemoKeysCheckBox.IsChecked);
            UpdateKeyDisplayerForm();
        }

        private void ResizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _settings.EnableResize(ResizeCheckBox.IsChecked);
            UpdateKeyDisplayerForm();
        }

        #endregion

        #region Keys

        private void FontComboBox_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (((ComboBoxItem)e.TargetObject).Content == FontComboBox.SelectedItem)
            {
                return;
            }
            e.Handled = true;
        }

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isFirstRun)
            {
                _settings.AddFontFamily(((FontFamily)FontComboBox.SelectedItem).Source);
                UpdateKeyDisplayerForm();
            }
        }

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(FontSizeTextBox.Text, out var number) && number >= 2 && number <= 1000)
            {
                _settings.FontSize = number;
            }
            else
            {
                var fontSizeDefault = new KeyDisplayerSettings().FontSize;
                _settings.FontSize = fontSizeDefault;
                FontSizeTextBox.Text = fontSizeDefault.ToString(CultureInfo.InvariantCulture);
            }
            UpdateKeyDisplayerForm();
        }

        private void DecreaseFontSizeRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var newValue = _settings.FontSize - 1;
            if (newValue >= 2)
            {
                _settings.FontSize = newValue;
                FontSizeTextBox.Text = newValue.ToString(CultureInfo.InvariantCulture);
                UpdateKeyDisplayerForm();
            }
        }

        private void IncreaseFontSizeRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var newValue = _settings.FontSize + 1;
            if (newValue <= 1000)
            {
                _settings.FontSize = newValue;
                FontSizeTextBox.Text = newValue.ToString(CultureInfo.InvariantCulture);
                UpdateKeyDisplayerForm();
            }
        }

        private void FontColorRectangle_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerWindow = new ColorPickerWindow(_settings.Color);
            colorPickerWindow.ShowDialog();
            _settings.Color = TransferModel.SelectedColor;
            FontColorRectangle.Background = new SolidColorBrush(TransferModel.SelectedColor);
            UpdateKeyDisplayerForm();
        }

        #endregion

        #region Footer

        private void ShowHideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_keyDisplayerForm.IsVisible)
            {
                _keyDisplayerForm.Close();
            }
            else
            {
                InitializeKeyDisplayerForm(_settings);
                _keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
            }
        }

        private void DefaultSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset your settings?", "YAKD - Default settings", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _settings = new KeyDisplayerSettings();
                _keyDisplayerForm.Close();
                _keyDisplayerForm = new KeyDisplayerForm(_settings);
                _keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
                InitializeMainWindow(_settings);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/Jagailo/YetAnotherKeyDisplayer/releases");
        }

        #endregion

        #endregion

        #region Window events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isRtssEnabled)
            {
                RTSSRadioButton.IsChecked = true;
            }
            else
            {
                WindowRadioButton.IsChecked = true;
            }

            MouseHookCheckBox.IsChecked = _isMouseEnabled;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Title += " [Saving ...]";
            _keyDisplayerForm.Close();
            SaveSettingsToFile(Properties.Settings.Default);
            if (RTSSHandler.IsRTSSRunning())
            {
                RTSSHandler.KillRTSS();
            }
        }

        private void KeyDisplayerForm_Closed(object sender, EventArgs e)
        {
            ShowHideWindowButton.Content = "Show";
        }

        private void KeyDisplayerForm_LocationChanged(object sender, EventArgs e)
        {
            _settings.StartupPoint = new StartupLocationModel(_keyDisplayerForm.Left, _keyDisplayerForm.Top);
        }

        private void KeyDisplayerForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _settings.Width = _keyDisplayerForm.Width;
            _settings.Height = _keyDisplayerForm.Height;
        }

        #endregion

        #region Events

        private void KeyboardHook_KeyDown(object sender, KeyboardHookEventArgs e) => AddKey(e.Key);

        private void KeyboardHook_KeyUp(object sender, KeyboardHookEventArgs e) => RemoveKey(e.Key);

        private void MouseHook_KeyDown(object sender, MouseHookEventArgs e) => AddKey(e.Key);

        private void MouseHook_KeyUp(object sender, MouseHookEventArgs e) => RemoveKey(e.Key);

        #endregion

        #region Helpers

        private void InitializeKeyDisplayerForm(KeyDisplayerSettings keyDisplayerSettings)
        {
            _keyDisplayerForm = new KeyDisplayerForm(keyDisplayerSettings);
            _keyDisplayerForm.LocationChanged += KeyDisplayerForm_LocationChanged;
            _keyDisplayerForm.SizeChanged += KeyDisplayerForm_SizeChanged;
            _keyDisplayerForm.Closed += KeyDisplayerForm_Closed;
        }

        private void UpdateKeyDisplayerForm()
        {
            if (_keyDisplayerForm != null && _keyDisplayerForm.IsVisible)
            {
                _keyDisplayerForm.InitializeSettings(_settings);
            }
        }

        private void InitializeMainWindow(KeyDisplayerSettings keyDisplayerSettings)
        {
            BackgroundColorRectangle.Background = new SolidColorBrush(keyDisplayerSettings.BackgroundColor);
            OpacityTextBox.Text = keyDisplayerSettings.BackgroundColorOpacity.ToString(CultureInfo.InvariantCulture);
            DemoKeysCheckBox.IsChecked = !string.IsNullOrEmpty(keyDisplayerSettings.DemoKeys);
            ResizeCheckBox.IsChecked = keyDisplayerSettings.CanResize;
            var font = FontComboBox.Items.Cast<FontFamily>().FirstOrDefault(x => x.ToString() == keyDisplayerSettings.FontFamily.Source);
            FontComboBox.SelectedIndex = font != null ? FontComboBox.Items.IndexOf(font) : 0;
            FontSizeTextBox.Text = keyDisplayerSettings.FontSize.ToString(CultureInfo.InvariantCulture);
            FontColorRectangle.Background = new SolidColorBrush(keyDisplayerSettings.Color);
        }

        private void InitializeKeyboardHook()
        {
            if (_keyboardHook == null)
            {
                _keyboardHook = new KeyboardHook();
                _keyboardHook.KeyDown += KeyboardHook_KeyDown;
                _keyboardHook.KeyUp += KeyboardHook_KeyUp;
            }
        }

        private void InitializeSettingsFromFile(Properties.Settings fileSettings)
        {
            if (fileSettings.Created)
            {
                try
                {
                    _settings.AddFontFamily(fileSettings.FontFamily);
                    _settings.FontSize = fileSettings.FontSize;
                    _settings.Color = fileSettings.Color;
                    _settings.BackgroundColor = fileSettings.BackgroundColor;
                    _settings.BackgroundColorOpacity = fileSettings.BackgroundColorOpacity;
                    _settings.EnableDemoKeys(fileSettings.DemoKeys);
                    if (fileSettings.StartupPoint)
                    {
                        _settings.StartupPoint = new StartupLocationModel(fileSettings.x, fileSettings.y);
                    }
                    _settings.Width = fileSettings.Width;
                    _settings.Height = fileSettings.Height;
                    _settings.EnableResize(fileSettings.CanResize);
                    RTSSHandler.RTSSPath = fileSettings.RTSSPath;
                    _isRtssEnabled = fileSettings.RTSSEnabled;
                    _isMouseEnabled = fileSettings.MouseEnabled;
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
        }

        private void SaveSettingsToFile(Properties.Settings fileSettings)
        {
            try
            {
                fileSettings.FontFamily = _settings.FontFamily.Source;
                fileSettings.FontSize = _settings.FontSize;
                fileSettings.Color = _settings.Color;
                fileSettings.BackgroundColor = _settings.BackgroundColor;
                fileSettings.BackgroundColorOpacity = _settings.BackgroundColorOpacity;
                fileSettings.DemoKeys = !string.IsNullOrEmpty(_settings.DemoKeys);
                if (_settings.StartupPoint != null)
                {
                    fileSettings.x = _settings.StartupPoint.Left;
                    fileSettings.y = _settings.StartupPoint.Top;
                    fileSettings.StartupPoint = true;
                }
                else
                {
                    fileSettings.x = 0;
                    fileSettings.y = 0;
                    fileSettings.StartupPoint = false;
                }
                fileSettings.Width = _settings.Width;
                fileSettings.Height = _settings.Height;
                fileSettings.CanResize = _settings.CanResize;
                fileSettings.RTSSEnabled = _isRtssEnabled;
                fileSettings.RTSSPath = RTSSHandler.RTSSPath;
                fileSettings.MouseEnabled = _isMouseEnabled;
                fileSettings.Created = true;
            }
            catch (Exception)
            {
                // Ignored
            }
            fileSettings.Save();
        }

        private void EnableControls(bool state)
        {
            BackgroundColorRectangle.IsEnabled =
                OpacitySlider.IsEnabled =
                OpacityTextBox.IsEnabled =
                DemoKeysCheckBox.IsEnabled =
                ResizeCheckBox.IsEnabled =
                FontComboBox.IsEnabled =
                FontSizeTextBox.IsEnabled =
                FontColorRectangle.IsEnabled =
                ShowHideWindowButton.IsEnabled =
                DefaultSettingsButton.IsEnabled =
                DecreaseFontSizeRepeatButton.IsEnabled =
                IncreaseFontSizeRepeatButton.IsEnabled = state;

            if (state && !_keyDisplayerForm.IsVisible)
            {
                InitializeKeyDisplayerForm(_settings);
                _keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
            }
            else if (!state && _keyDisplayerForm.IsVisible)
            {
                _keyDisplayerForm.Close();
            }
        }

        private void SendKeysToRTSS()
        {
            _keys.Sort((a, b) => b.Length.CompareTo(a.Length));
            var keysString = $" {string.Join(" + ", _keys)} ";

            try
            {
                RTSSHandler.Print(keysString);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddKey(string key)
        {
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                SendKeysToRTSS();
            }
        }

        private void RemoveKey(string key)
        {
            _keys.RemoveAll(x => x == key);
            SendKeysToRTSS();
        }

        private void EnableMouseHook()
        {
            _mouseHook = new MouseHook();
            _mouseHook.KeyDown += MouseHook_KeyDown;
            _mouseHook.KeyUp += MouseHook_KeyUp;
        }

        private void DisableMouseHook()
        {
            if (_mouseHook != null)
            {
                _mouseHook.Dispose();
                _mouseHook = null;
            }
        }

        #endregion
    }
}
