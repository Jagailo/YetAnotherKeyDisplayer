using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YAKD
{
    public partial class ColorPicker : UserControl
    {
        private bool isMouseDown, isChangingByUser;

        public ColorPicker()
        {
            InitializeComponent();
            SelectedColor = Colors.White;
            isMouseDown = false;
            isChangingByUser = true;
        }

        public void SetColor(Color color)
        {
            SelectedColor = color;
            UpdateTextBox();
            MoveEllipse(CanvImage.Width / 2, CanvImage.Height / 2, color);
        }

        public Color SelectedColor { get; private set; }

        private void UpdateTextBox()
        {
            string hexColor = "#";
            hexColor += Convert.ToString(SelectedColor.R, 16).Length == 1 ? "0" + Convert.ToString(SelectedColor.R, 16) : Convert.ToString(SelectedColor.R, 16);
            hexColor += Convert.ToString(SelectedColor.G, 16).Length == 1 ? "0" + Convert.ToString(SelectedColor.G, 16) : Convert.ToString(SelectedColor.G, 16);
            hexColor += Convert.ToString(SelectedColor.B, 16).Length == 1 ? "0" + Convert.ToString(SelectedColor.B, 16) : Convert.ToString(SelectedColor.B, 16);
            isChangingByUser = false;
            ColorName.Text = hexColor.ToUpper();
            isChangingByUser = true;
        }

        private void MoveEllipse(double x, double y, Color color)
        {
            ellipsePixel.SetValue(Canvas.LeftProperty, x - 5);
            ellipsePixel.SetValue(Canvas.TopProperty, y - 5);
            ellipsePixel.Fill = new SolidColorBrush(color);
        }

        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown)
            {
                return;
            }
            else if (Mouse.GetPosition(CanvasPanel).X < 5 || Mouse.GetPosition(CanvasPanel).X > 165 ||
                     Mouse.GetPosition(CanvasPanel).Y < 5 || Mouse.GetPosition(CanvasPanel).Y > 165)
            {
                isMouseDown = false;
            }
            else if (Mouse.GetPosition(CanvImage).X < CanvImage.Width && Mouse.GetPosition(CanvImage).X >= 0 &&
                     Mouse.GetPosition(CanvImage).Y < CanvImage.Height && Mouse.GetPosition(CanvImage).Y >= 0)
            {
                CroppedBitmap cb = new CroppedBitmap(ColorImage.Source as BitmapSource,
                    new Int32Rect((int)Mouse.GetPosition(CanvImage).X,
                    (int)Mouse.GetPosition(CanvImage).Y, 1, 1));

                byte[] pixels = new byte[4];
                cb.CopyPixels(pixels, 4, 0);
                MoveEllipse(Mouse.GetPosition(CanvImage).X, Mouse.GetPosition(CanvImage).Y, Color.FromArgb(255, pixels[2], pixels[1], pixels[0]));
                CanvImage.InvalidateVisual();

                SelectedColor = Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
                UpdateTextBox();
            }
        }

        private void CanvImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
        }

        private void CanvImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        private void ColorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isChangingByUser && Regex.IsMatch(ColorName.Text, @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
            {
                SelectedColor = (Color)ColorConverter.ConvertFromString(ColorName.Text);
                MoveEllipse(CanvImage.Width / 2, CanvImage.Height / 2, SelectedColor);
            }
        }
    }
}