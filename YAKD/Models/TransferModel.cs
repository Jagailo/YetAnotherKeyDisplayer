using System.Windows.Media;

namespace YAKD.Models
{
    /// <summary>
    /// Transfer model used only for transportation data between windows
    /// </summary>
    public static class TransferModel
    {
        #region Properties

        /// <summary>
        /// Selected color
        /// </summary>
        /// <remarks>Used in <see cref="ColorPickerWindow"/></remarks>
        public static Color SelectedColor { get; set; }

        /// <summary>
        /// RTSS path
        /// </summary>
        /// <remarks>Used in <see cref="RTSSWindow"/></remarks>
        public static string RTSSPath { get; set; }

        #endregion
    }
}
