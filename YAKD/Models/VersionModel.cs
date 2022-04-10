using Newtonsoft.Json;

namespace YAKD.Models
{
    /// <summary>
    /// Application version model
    /// </summary>
    public class VersionModel
    {
        #region Properties

        /// <summary>
        /// Version
        /// </summary>
        [JsonProperty("version")]
        public short Version { get; set; }

        #endregion
    }
}
