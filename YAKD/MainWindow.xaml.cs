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

        private KeyDisplayerForm keyDisplayerForm;
        private KeyDisplayerSettings settings;
        private bool isFirstRun, isSliderEnabled, isRTSSEnabled;
        private KeyboardHook keyboardHook;
        private List<string> keys;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MainWindow class
        /// </summary>
        public MainWindow()
        {
            settings = new KeyDisplayerSettings();
            isSliderEnabled = true;
            isRTSSEnabled = false;
            keys = new List<string>();
            InitializeSettingsFromFile(Properties.Settings.Default);
            isFirstRun = true;
            InitializeComponent();
            isFirstRun = false;
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeMainWindow(settings);
            InitializeKeyDisplayerForm(settings);
            keyDisplayerForm.Show();
            Focus();
        }

        #endregion

        #region Control handlers

        private void RTSSRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableDisableControls(false);
            RTSSRadioButton.IsChecked = true;
            e.Handled = true;
            isRTSSEnabled = true;
            RTSSHandler.RunRTSS();

            if (RTSSHandler.IsRTSSRunning())
            {
                InitializeKeyboardHook();
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

        private void OnHookKeyUp(object sender, KeyboardHookEventArgs e)
        {
            keys.RemoveAll(x => x == e.Key);
            SendKeysToRTSS();
        }

        private void OnHookKeyDown(object sender, KeyboardHookEventArgs e)
        {
            if (!keys.Exists(x => x == e.Key))
            {
                keys.Add(e.Key);
                SendKeysToRTSS();
            }
        }

        private void WindowRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableDisableControls(true);
            isRTSSEnabled = false;
            if (keyboardHook != null)
            {
                keyboardHook.Dispose();
                keyboardHook = null;
            }
        }

        private void BackgroundColorRectangle_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerWindow = new ColorPickerWindow(settings.BackgroundColor);
            colorPickerWindow.ShowDialog();
            settings.AddBackgroundColor(TransferModel.SelectedColor);
            BackgroundColorRectangle.Background = new SolidColorBrush(TransferModel.SelectedColor);
            UpdateKeyDisplayerForm();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OpacityTextBox != null && isSliderEnabled)
            {
                OpacityTextBox.Text = OpacitySlider.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void OpacityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = OpacityTextBox.Text.Replace('.', ',');
            if (double.TryParse(value, out var number) && number >= 0.01 && number <= 1)
            {
                settings.AddBackgroundColorOpacity(number);
                isSliderEnabled = false;
                OpacitySlider.Value = number;
                isSliderEnabled = true;
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
            settings.EnableDemoKeys(DemoKeysCheckBox.IsChecked);
            UpdateKeyDisplayerForm();
        }

        private void ResizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            settings.EnableResize(ResizeCheckBox.IsChecked);
            UpdateKeyDisplayerForm();
        }

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
            if (!isFirstRun)
            {
                settings.AddFontFamily(((FontFamily)FontComboBox.SelectedItem).Source);
                UpdateKeyDisplayerForm();
            }
        }

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(FontSizeTextBox.Text, out var number) && number >= 2 && number <= 1000)
            {
                settings.AddFontSize(number);
            }
            else
            {
                var fontSizeDefault = new KeyDisplayerSettings().FontSize;
                settings.AddFontSize(fontSizeDefault);
                FontSizeTextBox.Text = fontSizeDefault.ToString(CultureInfo.InvariantCulture);
            }
            UpdateKeyDisplayerForm();
        }

        private void FontColorRectangle_Click(object sender, RoutedEventArgs e)
        {
            var colorPickerWindow = new ColorPickerWindow(settings.Color);
            colorPickerWindow.ShowDialog();
            settings.AddColor(TransferModel.SelectedColor);
            FontColorRectangle.Background = new SolidColorBrush(TransferModel.SelectedColor);
            UpdateKeyDisplayerForm();
        }

        private void ShowHideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (keyDisplayerForm.IsVisible)
            {
                keyDisplayerForm.Close();
            }
            else
            {
                InitializeKeyDisplayerForm(settings);
                keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
            }
        }

        private void DefaultSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset your settings?", "YAKD - Default settings", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                settings = new KeyDisplayerSettings();
                keyDisplayerForm.Close();
                keyDisplayerForm = new KeyDisplayerForm(settings);
                keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
                InitializeMainWindow(settings);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/Jagailo/YetAnotherKeyDisplayer/releases");
        }

        private void DecreaseFontSizeRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var newValue = settings.FontSize - 1;
            if (newValue >= 2)
            {
                settings.AddFontSize(newValue);
                FontSizeTextBox.Text = newValue.ToString(CultureInfo.InvariantCulture);
                UpdateKeyDisplayerForm();
            }
        }

        private void IncreaseFontSizeRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var newValue = settings.FontSize + 1;
            if (newValue <= 1000)
            {
                settings.AddFontSize(newValue);
                FontSizeTextBox.Text = newValue.ToString(CultureInfo.InvariantCulture);
                UpdateKeyDisplayerForm();
            }
        }

        #endregion

        #region Form handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isRTSSEnabled)
            {
                RTSSRadioButton.IsChecked = true;
            }
            else
            {
                WindowRadioButton.IsChecked = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            keyDisplayerForm.Close();
            SaveSettingsToFile(Properties.Settings.Default);
            RTSSHandler.KillRTSS(); // TODO: придумать
        }

        private void KeyDisplayerForm_Closed(object sender, EventArgs e)
        {
            ShowHideWindowButton.Content = "Show";
        }

        private void KeyDisplayerForm_LocationChanged(object sender, EventArgs e)
        {
            settings.AddStartupPoint(new StartupLocationModel(keyDisplayerForm.Left, keyDisplayerForm.Top));
        }

        private void KeyDisplayerForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            settings.AddWidth(keyDisplayerForm.Width);
            settings.AddHeight(keyDisplayerForm.Height);
        }

        #endregion

        #region Initialization logic

        private void InitializeKeyDisplayerForm(KeyDisplayerSettings keyDisplayerSettings)
        {
            keyDisplayerForm = new KeyDisplayerForm(keyDisplayerSettings);
            keyDisplayerForm.LocationChanged += KeyDisplayerForm_LocationChanged;
            keyDisplayerForm.SizeChanged += KeyDisplayerForm_SizeChanged;
            keyDisplayerForm.Closed += KeyDisplayerForm_Closed;
        }

        private void UpdateKeyDisplayerForm()
        {
            if (keyDisplayerForm != null && keyDisplayerForm.IsVisible)
            {
                keyDisplayerForm.InitializeSettings(settings);
            }
        }

        private void InitializeMainWindow(KeyDisplayerSettings keyDisplayerSettings)
        {
            BackgroundColorRectangle.Background = new SolidColorBrush(keyDisplayerSettings.BackgroundColor);
            OpacityTextBox.Text = keyDisplayerSettings.BackgroundColorOpacity.ToString(CultureInfo.InvariantCulture);
            DemoKeysCheckBox.IsChecked = keyDisplayerSettings.DemoKeys != "";
            ResizeCheckBox.IsChecked = keyDisplayerSettings.CanResize;
            var font = FontComboBox.Items.Cast<FontFamily>().FirstOrDefault(x => x.ToString() == keyDisplayerSettings.FontFamily.Source);
            FontComboBox.SelectedIndex = FontComboBox.Items.IndexOf(font);
            FontSizeTextBox.Text = keyDisplayerSettings.FontSize.ToString(CultureInfo.InvariantCulture);
            FontColorRectangle.Background = new SolidColorBrush(keyDisplayerSettings.Color);
        }

        private void InitializeKeyboardHook()
        {
            if (keyboardHook == null)
            {
                keyboardHook = new KeyboardHook();
                keyboardHook.KeyDown += OnHookKeyDown;
                keyboardHook.KeyUp += OnHookKeyUp;
            }
        }

        private void InitializeSettingsFromFile(Properties.Settings fileSettings)
        {
            if (fileSettings.Created)
            {
                try
                {
                    settings.AddFontFamily(fileSettings.FontFamily);
                    settings.AddFontSize(fileSettings.FontSize);
                    settings.AddColor(fileSettings.Color);
                    settings.AddBackgroundColor(fileSettings.BackgroundColor);
                    settings.AddBackgroundColorOpacity(fileSettings.BackgroundColorOpacity);
                    settings.EnableDemoKeys(fileSettings.DemoKeys);
                    if (fileSettings.StartupPoint)
                    {
                        settings.AddStartupPoint(new StartupLocationModel(fileSettings.x, fileSettings.y));
                    }
                    settings.AddWidth(fileSettings.Width);
                    settings.AddHeight(fileSettings.Height);
                    settings.EnableResize(fileSettings.CanResize);
                    RTSSHandler.RTSSPath = fileSettings.RTSSPath;
                    isRTSSEnabled = fileSettings.RTSSEnabled;
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
                fileSettings.FontFamily = settings.FontFamily.Source;
                fileSettings.FontSize = settings.FontSize;
                fileSettings.Color = settings.Color;
                fileSettings.BackgroundColor = settings.BackgroundColor;
                fileSettings.BackgroundColorOpacity = settings.BackgroundColorOpacity;
                fileSettings.DemoKeys = settings.DemoKeys != "";
                if (settings.StartupPoint != null)
                {
                    fileSettings.x = settings.StartupPoint.Left;
                    fileSettings.y = settings.StartupPoint.Top;
                    fileSettings.StartupPoint = true;
                }
                else
                {
                    fileSettings.x = 0;
                    fileSettings.y = 0;
                    fileSettings.StartupPoint = false;
                }
                fileSettings.Width = settings.Width;
                fileSettings.Height = settings.Height;
                fileSettings.CanResize = settings.CanResize;
                fileSettings.RTSSEnabled = isRTSSEnabled;
                fileSettings.RTSSPath = RTSSHandler.RTSSPath;
                fileSettings.Created = true;
            }
            catch (Exception)
            {
                // Ignored
            }
            fileSettings.Save();
        }

        private void EnableDisableControls(bool state)
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
                DefaultSettingsButton.IsEnabled = state;

            if (state && !keyDisplayerForm.IsVisible)
            {
                InitializeKeyDisplayerForm(settings);
                keyDisplayerForm.Show();
                ShowHideWindowButton.Content = "Hide (Alt + F4)";
            }
            else if (!state && keyDisplayerForm.IsVisible)
            {
                keyDisplayerForm.Close();
            }
        }

        #endregion

        #region Logic

        private void SendKeysToRTSS()
        {
            keys.Sort((a, b) => b.Length.CompareTo(a.Length));
            var keysString = $" {string.Join(" + ", keys)} ";

            try
            {
                RTSSHandler.Print(keysString);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
