using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YAKD.Helpers;
using YAKD.Hooks.Keyboard;
using YAKD.Hooks.Mouse;

namespace YAKD
{
    public partial class KeyDisplayerForm : Window
    {
        private KeyboardHook keyboardHook;
        private MouseHook mouseHook;
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
            keyboardHook.KeyDown += OnHookKeyDown;
            keyboardHook.KeyUp += OnHookKeyUp;

            mouseHook = new MouseHook();
            mouseHook.KeyDown += OnHookMouseKeyDown;
            mouseHook.KeyUp += OnHookMouseKeyUp;

            keys = new List<string>();
        }

        public void InitializeSettings(KeyDisplayerSettings settings)
        {
            keysTextBlock.FontFamily = settings.FontFamily;
            keysTextBlock.FontSize = settings.FontSize;
            keysTextBlock.Foreground = new SolidColorBrush(settings.Color);
            var solidColor = new SolidColorBrush(settings.BackgroundColor)
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
            ResizeMode = settings.CanResize ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;
        }

        private void OnHookKeyUp(object sender, KeyboardHookEventArgs e)
        {
            keys.RemoveAll(x => x == e.Key);
            ShowKeys();
        }

        private void OnHookKeyDown(object sender, KeyboardHookEventArgs e)
        {
            if (!keys.Exists(x => x == e.Key))
            {
                keys.Add(e.Key);
                ShowKeys();
            }
        }

        private void ShowKeys()
        {
            if (isKeyboardHookEnable) // TODO: mouse 
            {
                // TODO: string join
                keys.Sort((a, b) => b.Length.CompareTo(a.Length));
                keysTextBlock.Text = "";
                for (var i = 0; i < keys.Count; i++)
                {
                    keysTextBlock.Text += keys.ElementAt(i);
                    if (i != keys.Count - 1)
                    {
                        keysTextBlock.Text += " + ";
                    }
                }
            }
        }

        private void OnHookMouseKeyUp(object sender, MouseHookEventArgs e)
        {
            keys.RemoveAll(x => x == e.Key);
            ShowKeys();
        }

        private void OnHookMouseKeyDown(object sender, MouseHookEventArgs e)
        {
            if (keys.Exists(x => x == e.Key)) return;
            keys.Add(e.Key);
            ShowKeys();
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}