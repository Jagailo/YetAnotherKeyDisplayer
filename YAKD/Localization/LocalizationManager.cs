using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using YAKD.Models;

namespace YAKD.Localization
{
    /// <summary>
    /// Manages the interface language and provides localized strings for bindings.
    /// </summary>
    public sealed class LocalizationManager : INotifyPropertyChanged
    {
        #region Fields

        private static readonly LanguageModel DefaultLanguage = new LanguageModel("en-US", "English");

        private readonly CultureInfo _systemUiCulture;

        #endregion

        #region Properties

        /// <summary>
        /// Single instance of the manager.
        /// </summary>
        public static LocalizationManager Instance { get; } = new LocalizationManager();

        /// <summary>
        /// Languages available in the interface.
        /// </summary>
        public IReadOnlyList<LanguageModel> SupportedLanguages { get; }

        /// <summary>
        /// Current interface language.
        /// </summary>
        public LanguageModel CurrentLanguage { get; private set; }

        /// <summary>
        /// Gets the localized string for the current language.
        /// </summary>
        /// <param name="key">Resource key.</param>
        /// <returns>Localized string or the key itself if the resource is missing.</returns>
        public string this[string key] => Strings.ResourceManager.GetString(key, Strings.Culture) ?? key;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs after the interface language has been changed.
        /// </summary>
        public event EventHandler LanguageChanged;

        #endregion

        #region Constructor

        private LocalizationManager()
        {
            // Captured before SetLanguage overrides the UI culture of the process.
            _systemUiCulture = CultureInfo.CurrentUICulture;

            // Ordered by native name: Latin script alphabetically, then Cyrillic, then CJK.
            SupportedLanguages = new[]
            {
                new LanguageModel("de", "Deutsch"),
                DefaultLanguage,
                new LanguageModel("es", "Español"),
                new LanguageModel("fr", "Français"),
                new LanguageModel("it", "Italiano"),
                new LanguageModel("pl", "Polski"),
                new LanguageModel("pt-BR", "Português"),
                new LanguageModel("tr", "Türkçe"),
                new LanguageModel("ru", "Русский"),
                new LanguageModel("ja", "日本語"),
                new LanguageModel("zh-Hans", "中文"),
                new LanguageModel("ko", "한국어")
            };

            CurrentLanguage = DefaultLanguage;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the saved language or, if it is not set or not supported, the Windows display language or the default one.
        /// </summary>
        /// <param name="cultureName">Saved culture name.</param>
        public void Initialize(string cultureName)
        {
            SetLanguage(FindLanguage(cultureName) ?? FindSystemLanguage() ?? DefaultLanguage);
        }

        /// <summary>
        /// Restores the Windows display language or, if it is not supported, the default one.
        /// </summary>
        public void ResetLanguage()
        {
            SetLanguage(FindSystemLanguage() ?? DefaultLanguage);
        }

        /// <summary>
        /// Changes the interface language.
        /// </summary>
        /// <param name="language">New language.</param>
        public void SetLanguage(LanguageModel language)
        {
            var culture = CultureInfo.GetCultureInfo(language.CultureName);
            Strings.Culture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CurrentLanguage = language;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Binding.IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Helpers

        private LanguageModel FindLanguage(string cultureName)
        {
            return SupportedLanguages.FirstOrDefault(x => string.Equals(x.CultureName, cultureName, StringComparison.OrdinalIgnoreCase));
        }

        private LanguageModel FindSystemLanguage()
        {
            for (var culture = _systemUiCulture; !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
            {
                var language = FindLanguage(culture.Name);
                if (language != null)
                {
                    return language;
                }
            }

            var languageCode = _systemUiCulture.TwoLetterISOLanguageName;

            return SupportedLanguages.FirstOrDefault(x => CultureInfo.GetCultureInfo(x.CultureName).TwoLetterISOLanguageName == languageCode);
        }

        #endregion
    }
}
