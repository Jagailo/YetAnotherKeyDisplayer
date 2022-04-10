using Newtonsoft.Json;

namespace YAKD.Models
{
    /// <summary>
    /// Configuration model
    /// </summary>
    public class ConfigModel
    {
        #region Properties

        /// <summary>
        /// Statistics URL
        /// </summary>
        [JsonProperty("statUrl")]
        public string StatUrl { get; set; }

        #endregion
    }
}
