namespace YAKD.Models
{
    /// <summary>
    /// Represents an interface language.
    /// </summary>
    public class LanguageModel
    {
        #region Properties

        /// <summary>
        /// Culture name of the language resources (for example, "en-US" or "zh-Hans").
        /// </summary>
        public string CultureName { get; }

        /// <summary>
        /// Language name written in the language itself.
        /// </summary>
        public string NativeName { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LanguageModel class.
        /// </summary>
        /// <param name="cultureName">Culture name of the language resources.</param>
        /// <param name="nativeName">Language name written in the language itself.</param>
        public LanguageModel(string cultureName, string nativeName)
        {
            CultureName = cultureName;
            NativeName = nativeName;
        }

        #endregion
    }
}
