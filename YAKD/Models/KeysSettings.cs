namespace YAKD.Models
{
    /// <summary>
    /// Keys settings
    /// </summary>
    public class KeysSettings
    {
        /// <summary>
        /// Mouse hook enabled setting
        /// </summary>
        public bool IsMouseEnabled { get; set; }

        /// <summary>
        /// Short name for Numpad
        /// </summary>
        public bool ShortNameForNumpad { get; set; }

        /// <summary>
        /// Ignore Left (L) and Right (R) prefixes
        /// </summary>
        public bool IgnoreLeftRight { get; set; }
    }
}
