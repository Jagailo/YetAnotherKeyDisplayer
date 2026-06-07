namespace YAKD.Models
{
    /// <summary>
    /// Represents window location settings.
    /// </summary>
    public class StartupLocationModel
    {
        #region Properties

        /// <summary>
        /// Distance from the top of the screen.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Distance from the left of the screen.
        /// </summary>
        public double Left { get; set; }

        #endregion
    }
}
