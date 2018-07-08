using System.Windows;
using System.Windows.Media;
using YAKD.Utils;

namespace YAKD
{
    public partial class ColorPickerWindow : Window
    {
        public ColorPickerWindow(Color color)
        {
            InitializeComponent();
            ColorPickerControl.SetColor(color);
            Transfer.SelectedColor = color;
            OKButton.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Transfer.SelectedColor = ColorPickerControl.SelectedColor;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}