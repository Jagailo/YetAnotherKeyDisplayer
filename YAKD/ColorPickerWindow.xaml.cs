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
            ColorPickerControl.SetColor(color);
        }

        #endregion

        #region Handlers

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            TransferModel.SelectedColor = ColorPickerControl.SelectedColor;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
