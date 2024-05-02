using System.Windows;
using System.Windows.Media;
using YAKD.Models;

namespace YAKD
{
    /// <summary>
    /// Color picker window
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ColorPickerWindow class
        /// </summary>
        /// <param name="color">Initial color</param>
        public ColorPickerWindow(Color color)
        {
            InitializeComponent();
            OkButton.Focus();

            TransferModel.SelectedColor = color;
            if (color == Colors.Black)
            {
                ColorCanvas.HexadecimalString = "#000000";
            }
            else
            {
                ColorCanvas.R = color.R;
                ColorCanvas.G = color.G;
                ColorCanvas.B = color.B;
            }
        }

        #endregion

        #region Handlers

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            TransferModel.SelectedColor = Color.FromRgb(ColorCanvas.R, ColorCanvas.G, ColorCanvas.B);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
