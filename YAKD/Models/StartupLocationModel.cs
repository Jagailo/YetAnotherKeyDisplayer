namespace YAKD.Models
{
    /// <summary>
    /// Entity which represents window location settings
    /// </summary>
    public class StartupLocationModel
    {
        #region Fields

        /// <summary>
        /// Point from top of the screen
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Point from left of the screen
        /// </summary>
        public double Left { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of StartupLocation class
        /// </summary>
        public StartupLocationModel(double x, double y)
        {
            Left = x;
            Top = y;
        }

        #endregion
    }
}
