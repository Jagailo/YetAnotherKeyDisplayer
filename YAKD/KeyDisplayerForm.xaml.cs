using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YAKD.Utils;

namespace YAKD
{
    public partial class KeyDisplayerForm : Window
    {
        private KeyboardHook keyboardHook;
        private bool isKeyboardHookEnable;
        private List<string> keys;

        public KeyDisplayerForm(KeyDisplayerSettings settings)
        {
            if (settings == null)
            {
                settings = new KeyDisplayerSettings();
            }

            InitializeComponent();
            InitializeSettings(settings);

            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);
            keyboardHook.KeyUp += new KeyboardHook.HookEventHandler(OnHookKeyUp);

            keys = new List<string>();
        }

        public void InitializeSettings(KeyDisplayerSettings settings)
        {
            keysTextBlock.FontFamily = settings.FontFamily;
            keysTextBlock.FontSize = settings.FontSize;
            keysTextBlock.Foreground = new SolidColorBrush(settings.Color);
            SolidColorBrush solidColor = new SolidColorBrush(settings.BackgroundColor)
            {
                Opacity = settings.BackgroundColorOpacity
            };
            Background = solidColor;
            if (settings.DemoKeys != "")
            {
                keysTextBlock.Text = settings.DemoKeys;
                isKeyboardHookEnable = false;
            }
            else
            {
                keysTextBlock.Text = "";
                isKeyboardHookEnable = true;
            }            
            if (settings.StartupPoint != null)
            {
                Top = settings.StartupPoint.Top;
                Left = settings.StartupPoint.Left;
            }
            Height = settings.Height;
            Width = settings.Width;
            if (settings.CanResize)
            {
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                ResizeMode = ResizeMode.NoResize;
            }
        }

        private void OnHookKeyUp(object sender, HookEventArgs e)
        {
            keys.RemoveAll(x => x == e.Key);
            ShowKeys();
        }

        private void OnHookKeyDown(object sender, HookEventArgs e)
        {
            if (!keys.Exists(x => x == e.Key))
            {
                keys.Add(e.Key);
                ShowKeys();
            }
        }

        private void ShowKeys()
        {
            if (isKeyboardHookEnable)
            {
                keys.Sort((a, b) => b.Length.CompareTo(a.Length));
                keysTextBlock.Text = "";
                for (int i = 0; i < keys.Count; i++)
                {
                    keysTextBlock.Text += keys.ElementAt(i);
                    if (i != keys.Count - 1)
                    {
                        keysTextBlock.Text += " + ";
                    }
                }
            }
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}