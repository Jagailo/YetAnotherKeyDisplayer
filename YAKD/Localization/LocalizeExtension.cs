using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace YAKD.Localization
{
    /// <summary>
    /// XAML markup extension that binds a property to a localized string and updates it when the language changes.
    /// </summary>
    /// <example><c>Content="{loc:Localize MainWindow_TrackMouseCheckBox}"</c></example>
    [MarkupExtensionReturnType(typeof(object))]
    public class LocalizeExtension : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Resource key.
        /// </summary>
        [ConstructorArgument("key")]
        public string Key { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LocalizeExtension class.
        /// </summary>
        public LocalizeExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LocalizeExtension class.
        /// </summary>
        /// <param name="key">Resource key.</param>
        public LocalizeExtension(string key)
        {
            Key = key;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a binding to the localized string.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <returns>Binding value for the target property.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding($"[{Key}]") { Source = LocalizationManager.Instance, Mode = BindingMode.OneWay };

            return binding.ProvideValue(serviceProvider);
        }

        #endregion
    }
}
