namespace YAKD.Models
{
    /// <summary>
    /// Key model
    /// </summary>
    public class KeyModel
    {
        #region Properties

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the KeyModel class
        /// </summary>
        /// <param name="name">Key original name</param>
        /// <param name="settings">Settings</param>
        public KeyModel(string name, KeysSettings settings)
        {
            Name = name;
            DisplayName = UppercaseFirstLetter(name);

            if (settings != null)
            {
                if (settings.ShortNameForNumpad)
                {
                    DisplayName = DisplayName.Replace("Numpad", "Pad");
                }

                if (settings.IgnoreLeftRight)
                {
                    DisplayName = DisplayName
                        .Replace("L ", string.Empty)
                        .Replace("Left ", string.Empty)
                        .Replace("R ", string.Empty)
                        .Replace("Right ", string.Empty);
                }

                if (settings.UseArrowIcons)
                {
                    switch (Name)
                    {
                        case "Up":
                        {
                            DisplayName = DisplayName.Replace("Up", "\u2191"); // ↑

                            break;
                        }
                        case "Right":
                        {
                            DisplayName = DisplayName.Replace("Right", "\u2192"); // →

                            break;
                        }
                        case "Down":
                        {
                            DisplayName = DisplayName.Replace("Down", "\u2193"); // ↓

                            break;
                        }
                        case "Left":
                        {
                            DisplayName = DisplayName.Replace("Left", "\u2190"); // ←

                            break;
                        }
                    }
                }

                DisplayName = UppercaseFirstLetter(DisplayName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the KeyModel class.
        /// </summary>
        /// <param name="name">Key original name</param>
        public KeyModel(string name)
        {
            Name = name;
            DisplayName = UppercaseFirstLetter(name);
        }

        #endregion

        #region Helpers

        private static string UppercaseFirstLetter(string value)
        {
            if (char.IsUpper(value[0]))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToUpper();
            }

            return char.ToUpper(value[0]) + value.Substring(1);
        }

        #endregion
    }
}
