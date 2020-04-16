using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using YAKD.Models;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace YAKD
{
    /// <summary>
    /// Windows with RTSS error
    /// </summary>
    public partial class RTSSWindow : Window
    {
        #region Fields

        private readonly FolderBrowserDialog _dialog;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the RTSSWindow class
        /// </summary>
        /// <param name="RTSSPath">Path to RTSS applications</param>
        public RTSSWindow(string RTSSPath)
        {
            InitializeComponent();
            _dialog = new FolderBrowserDialog
            {
                Description = "Specify the directory with the 'RTSS.exe' file.",
                RootFolder = System.Environment.SpecialFolder.ProgramFilesX86,
                ShowNewFolderButton = false
            };
            RTSSPathTextBlock.Text = $"In the directory \"{RTSSPath.Remove(RTSSPath.LastIndexOf('\\'), RTSSPath.Length - RTSSPath.LastIndexOf('\\'))}\" could not find RTSS.exe.";
        }

        #endregion

        #region Handlers

        private void DownloadRTSSButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.guru3d.com/files-details/rtss-rivatuner-statistics-server-download.html");
            Close();
        }

        private void FindRTSSButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog.ShowDialog() == MessageBoxResult.OK)
            {
                var path = Path.Combine(_dialog.SelectedPath, "RTSS.exe");
                if (File.Exists(path))
                {
                    TransferModel.RTSSPath = path;
                    Close();
                }
                else
                {
                    MessageBox.Show("There is no RTSS.exe file in this directory", "RTSS.exe not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        #endregion
    }
}
