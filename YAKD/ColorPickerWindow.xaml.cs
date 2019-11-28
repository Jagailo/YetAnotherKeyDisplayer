using System.Windows;
using System.Windows.Media;
using YAKD.Models;

namespace YAKD
{
    public partial class ColorPickerWindow : Window
    {
        public ColorPickerWindow(Color color)
        {
            InitializeComponent();
            ColorPickerControl.SetColor(color);
            TransferModel.SelectedColor = color;
            OKButton.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            TransferModel.SelectedColor = ColorPickerControl.SelectedColor;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}