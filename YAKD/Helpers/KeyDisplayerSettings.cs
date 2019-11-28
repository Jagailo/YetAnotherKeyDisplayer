using System.Windows.Media;
using YAKD.Models;

namespace YAKD.Helpers
{
    /// <summary>
    /// Settings for KeyDisplayer (window)
    /// </summary>
    public class KeyDisplayerSettings
    {
        #region Fields

        /// <summary>
        /// Font
        /// </summary>
        public FontFamily FontFamily { get; private set; }

        /// <summary>
        /// Font size
        /// </summary>
        /// <remarks>From 2 to 1000</remarks>
        public double FontSize { get; private set; }

        /// <summary>
        /// Font color
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Background color opacity
        /// </summary>
        public double BackgroundColorOpacity { get; private set; }

        /// <summary>
        /// List of demo keys
        /// </summary>
        public string DemoKeys { get; private set; }

        /// <summary>
        /// Startup position
        /// </summary>
        public StartupLocationModel StartupPoint { get; private set; }

        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Is resizable
        /// </summary>
        public bool CanResize { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of KeyDisplayerSettings class with default settings
        /// </summary>
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

        #endregion

        #region Methods

        /// <summary>
        /// Updates font
        /// </summary>
        /// <param name="fontName">Font family name</param>
        public void AddFontFamily(string fontName)
        {
            FontFamily = new FontFamily(fontName);
        }

        /// <summary>
        /// Updates font size
        /// </summary>
        /// <param name="fontSize">Font size</param>
        /// <returns>True if font size was successfully updated; otherwise, False</returns>
        public bool AddFontSize(double fontSize)
        {
            if (fontSize > 1 && fontSize <= 1000)
            {
                FontSize = fontSize;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates font color
        /// </summary>
        /// <param name="color"><see cref="Color"/></param>
        public void AddColor(Color color)
        {
            Color = color;
        }

        /// <summary>
        /// Updates background color
        /// </summary>
        /// <param name="backgroundColor"><see cref="Color"/></param>
        public void AddBackgroundColor(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// Updates background color opacity
        /// </summary>
        /// <param name="backgroundColorOpacity">Color opacity</param>
        /// <returns>True if color opacity was successfully updated; otherwise, False</returns>
        public bool AddBackgroundColorOpacity(double backgroundColorOpacity)
        {
            if (backgroundColorOpacity >= 0.01 && backgroundColorOpacity <= 1)
            {
                BackgroundColorOpacity = backgroundColorOpacity;
                return true;
            }

            return false;
        }

        // TODO: doc
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

        /// <summary>
        /// Updates window startup point
        /// </summary>
        /// <param name="startupPoint"><see cref="StartupPoint"/></param>
        public void AddStartupPoint(StartupLocationModel startupPoint)
        {
            StartupPoint = startupPoint;
        }

        /// <summary>
        /// Updates window height
        /// </summary>
        /// <param name="height">Height</param>
        /// <returns><returns>True if height was successfully updated; otherwise, False</returns></returns>
        public bool AddHeight(double height)
        {
            if (height >= 20)
            {
                Height = height;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates window width
        /// </summary>
        /// <param name="width">Width</param>
        /// <returns><returns>True if width was successfully updated; otherwise, False</returns></returns>
        public bool AddWidth(double width)
        {
            if (width >= 20)
            {
                Width = width;
                return true;
            }

            return false;
        }

        // TODO: doc
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

        #endregion
    }
}
