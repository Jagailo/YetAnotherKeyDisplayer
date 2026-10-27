using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using YAKD.Localization;
using YAKD.Models;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace YAKD
{
    /// <summary>
    /// RTSS error window.
    /// </summary>
    public partial class RTSSWindow : Window
    {
        #region Fields

        private readonly FolderBrowserDialog _dialog;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RTSSWindow class.
        /// </summary>
        /// <param name="RTSSPath">Path to the RTSS application.</param>
        public RTSSWindow(string RTSSPath)
        {
            InitializeComponent();
            _dialog = new FolderBrowserDialog
            {
                Description = Strings.RtssWindow_FolderDialogDescription,
                RootFolder = System.Environment.SpecialFolder.ProgramFilesX86,
                ShowNewFolderButton = false
            };

            RTSSPathTextBlock.Text = string.Format(Strings.RtssWindow_NotFoundInDirectoryFormat, Path.GetDirectoryName(RTSSPath));
        }

        #endregion

        #region Handlers

        private void DownloadRTSSButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.guru3d.com/download/rtss-rivatuner-statistics-server-download");
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
                    MessageBox.Show(Strings.RtssWindow_FileNotFoundMessage, Strings.RtssWindow_FileNotFoundCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        #endregion
    }
}
