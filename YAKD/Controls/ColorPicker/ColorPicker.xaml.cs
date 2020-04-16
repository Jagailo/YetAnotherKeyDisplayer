using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YAKD.Controls.ColorPicker
{
    /// <summary>
    /// Color picker control
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region Fields

        private bool _isMouseDown, _isChangingByUser;

        private readonly double _ellipseHalfWidth;

        private readonly double _ellipseHalfHeight;

        #endregion

        #region Properties

        /// <summary>
        /// Selected color
        /// </summary>
        public Color SelectedColor { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ColorPicker class
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();

            _ellipseHalfWidth = EllipsePixel.Width / 2.0;
            _ellipseHalfHeight = EllipsePixel.Height / 2.0;

            SetColor(Colors.White);
            _isChangingByUser = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets new color
        /// </summary>
        /// <param name="color">New color</param>
        public void SetColor(Color color)
        {
            SelectedColor = color;
            MoveEllipse(CanvasImage.Width / 2.0, CanvasImage.Height / 2.0, color);
            UpdateTextBox();
            ColorRectangle.Fill = new SolidColorBrush(color);
        }

        #endregion

        #region Handlers

        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var x = Mouse.GetPosition(CanvasPanel).X;
                var y = Mouse.GetPosition(CanvasPanel).Y;

                if (x < _ellipseHalfWidth || x > CanvasPanel.Width - _ellipseHalfWidth || y < _ellipseHalfHeight || y > CanvasPanel.Height - _ellipseHalfHeight)
                {
                    _isMouseDown = false;
                }
                else if (x < CanvasImage.Width && x >= 0 && y < CanvasImage.Height && y >= 0)
                {
                    try
                    {
                        var croppedBitmap = new CroppedBitmap(ColorImage.Source as BitmapSource, new Int32Rect(Convert.ToInt32(x), Convert.ToInt32(y), 1, 1));
                        var pixels = new byte[4];
                        croppedBitmap.CopyPixels(pixels, 4, 0);
                        SelectedColor = Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
                        MoveEllipse(x, y, SelectedColor);
                        UpdateTextBox();
                        ColorRectangle.Fill = new SolidColorBrush(SelectedColor);
                    }
                    catch (Exception)
                    {
                        _isMouseDown = false;
                        SetColor(Colors.White);
                    }
                    finally
                    {
                        CanvasImage.InvalidateVisual();
                    }
                }
            }
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void CanvasImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        private void ColorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isChangingByUser && Regex.IsMatch(ColorName.Text, @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
            {
                SelectedColor = (Color)ColorConverter.ConvertFromString(ColorName.Text);
                MoveEllipse(CanvasImage.Width / 2.0, CanvasImage.Height / 2.0, SelectedColor);
                ColorRectangle.Fill = new SolidColorBrush(SelectedColor);
            }
        }

        #endregion

        #region Helpers

        private void UpdateTextBox()
        {
            var r = Convert.ToString(SelectedColor.R, 16);
            var g = Convert.ToString(SelectedColor.G, 16);
            var b = Convert.ToString(SelectedColor.B, 16);

            var hexColor = new StringBuilder("#");
            hexColor.Append(r.Length == 1 ? $"0{r}" : r);
            hexColor.Append(g.Length == 1 ? $"0{g}" : g);
            hexColor.Append(b.Length == 1 ? $"0{b}" : b);

            _isChangingByUser = false;
            ColorName.Text = hexColor.ToString().ToUpper();
            _isChangingByUser = true;
        }

        private void MoveEllipse(double x, double y, Color color)
        {
            EllipsePixel.SetValue(Canvas.LeftProperty, x - _ellipseHalfWidth);
            EllipsePixel.SetValue(Canvas.TopProperty, y - _ellipseHalfHeight);
            EllipsePixel.Fill = new SolidColorBrush(color);
        }

        #endregion
    }
}
