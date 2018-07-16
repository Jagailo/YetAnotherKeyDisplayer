using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using YAKD.Utils;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace YAKD
{
    public partial class RTSSWindow : Window
    {
        private FolderBrowserDialog dialog;

        public RTSSWindow(string RTSSPath)
        {
            InitializeComponent();
            dialog = new FolderBrowserDialog
            {
                Description = "Specify the directory with the 'RTSS.exe' file.",
                RootFolder = System.Environment.SpecialFolder.ProgramFilesX86,
                ShowNewFolderButton = false
            };
            RTSSPathTextBlock.Text = $"In the directory \"{RTSSPath.Remove(RTSSPath.LastIndexOf('\\'), RTSSPath.Length - RTSSPath.LastIndexOf('\\'))}\" could not find RTSS.exe.";
        }

        private void DownloadRTSSButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.guru3d.com/files-details/rtss-rivatuner-statistics-server-download.html");
            Close();
        }

        private void FindRTSSButton_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == MessageBoxResult.OK)
            {
                string path = Path.Combine(dialog.SelectedPath, "RTSS.exe");
                if (File.Exists(path))
                {
                    Transfer.RTSSPath = path;
                    Close();
                }
                else
                {
                    MessageBox.Show("There is no RTSS.exe file in this directory", "RTSS.exe not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}