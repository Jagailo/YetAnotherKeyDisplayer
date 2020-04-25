﻿using System.Windows;
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

        private double _fontSize, _backgroundColorOpacity, _height, _width;

        #endregion

        #region Properties

        /// <summary>
        /// Font
        /// </summary>
        public FontFamily FontFamily { get; private set; }

        /// <summary>
        /// Font size
        /// </summary>
        /// <remarks>From 2 to 1000</remarks>
        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (value > 1 && value <= 1000)
                {
                    _fontSize = value;
                }
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Background color opacity
        /// </summary>
        public double BackgroundColorOpacity
        {
            get => _backgroundColorOpacity;
            set
            {
                if (value >= 0.01 && value <= 1)
                {
                    _backgroundColorOpacity = value;
                }
            }
        }

        /// <summary>
        /// List of demo keys
        /// </summary>
        public string DemoKeys { get; private set; }

        /// <summary>
        /// Startup position
        /// </summary>
        public StartupLocationModel StartupPoint { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public double Height
        {
            get => _height;
            set
            {
                if (value >= 20)
                {
                    _height = value;
                }
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                if (value >= 20)
                {
                    _width = value;
                }
            }
        }

        /// <summary>
        /// Is resizable
        /// </summary>
        public bool CanResize { get; private set; }

        /// <summary>
        /// Is tracking mouse
        /// </summary>
        public bool MouseEnabled { get; set; }

        /// <summary>
        /// Horizontal keys alignment
        /// </summary>
        public HorizontalAlignment KeysAlignment { get; set; }

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
            DemoKeys = string.Empty;
            Height = 70;
            Width = 300;
            KeysAlignment = HorizontalAlignment.Center;
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
        /// Enables demo keys
        /// </summary>
        /// <param name="value">Value</param>
        public void EnableDemoKeys(bool? value)
        {
            if (value.HasValue && value.Value)
            {
                DemoKeys = "L Shift + L Ctrl + Enter + C + V";
            }
            else
            {
                DemoKeys = "";
            }
        }

        /// <summary>
        /// Enables resize option
        /// </summary>
        /// <param name="value">Value</param>
        public void EnableResize(bool? value)
        {
            if (value.HasValue && value.Value)
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
