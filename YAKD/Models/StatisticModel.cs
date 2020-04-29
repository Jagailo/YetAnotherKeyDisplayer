using YAKD.Enums;

namespace YAKD.Models
{
    /// <summary>
    /// Statistic model
    /// </summary>
    public class StatisticModel
    {
        #region Properties

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Statistic type
        /// </summary>
        /// <value>0 - Installation</value>
        public StatisticType Type { get; set; }

        #endregion
    }
}
