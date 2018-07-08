using System.Windows.Media;

namespace YAKD.Utils
{
    public class KeyDisplayerSettings
    {
        public KeyDisplayerSettings()
        {
            FontFamily = new FontFamily("Arial");
            FontSize = 16;
            Color = Color.FromRgb(238, 238, 238);
            BackgroundColor = Colors.Black;
            BackgroundColorOpacity = 0.4;
            DemoKeys = "";
            StartupPoint = null;
            Height = 70;
            Width = 300;
            CanResize = false;
        }

        public void AddFontFamily(string fontName)
        {
            FontFamily = new FontFamily(fontName);
        }

        public bool AddFontSize(double fontSize)
        {
            if (fontSize > 1 && fontSize <= 1000)
            {
                FontSize = fontSize;
                return true;
            }
            return false;
        }

        public void AddColor(Color color)
        {
            Color = color;
        }

        public void AddBackgroundColor(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        public bool AddBackgroundColorOpacity(double backgroundColorOpacity)
        {
            if (backgroundColorOpacity >= 0.01 && backgroundColorOpacity <= 1)
            {
                BackgroundColorOpacity = backgroundColorOpacity;
                return true;
            }
            return false;
        }

        public void EnableDemoKeys(bool? value)
        {
            if (value.Value)
            {
                DemoKeys = "L Shift + L Ctrl + Enter + C + V";
            }
            else
            {
                DemoKeys = "";
            }
        }

        public void AddStartupPoint(StartupLocation startupPoint)
        {
            StartupPoint = startupPoint;
        }

        public bool AddHeight(double height)
        {
            if (height >= 20)
            {
                Height = height;
                return true;
            }
            return false;
        }

        public bool AddWidth(double width)
        {
            if (width >= 20)
            {
                Width = width;
                return true;
            }
            return false;
        }

        public void EnableResize(bool? value)
        {
            if (value.Value)
            {
                CanResize = true;
            }
            else
            {
                CanResize = false;
            }
        }

        public FontFamily FontFamily { get; private set; }
        public double FontSize { get; private set; }
        public Color Color { get; private set; }
        public Color BackgroundColor { get; private set; }
        public double BackgroundColorOpacity { get; private set; }
        public string DemoKeys { get; private set; }
        public StartupLocation StartupPoint { get; private set; }
        public double Height { get; private set; }
        public double Width { get; private set; }
        public bool CanResize { get; private set; }
    }

    public class StartupLocation
    {
        public StartupLocation(double x, double y)
        {
            Left = x;
            Top = y;
        }

        public double Top { get; private set; }
        public double Left { get; private set; }
    }
}