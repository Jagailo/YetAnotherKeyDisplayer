using Newtonsoft.Json;

namespace YAKD.Models
{
    /// <summary>
    /// Application version model.
    /// </summary>
    public class VersionModel
    {
        #region Properties

        /// <summary>
        /// Latest released version in the "major.minor.build" format.
        /// </summary>
        /// <remarks>The numeric "version" field of the same file is kept only for versions up to 1.5 and is not read here.</remarks>
        [JsonProperty("latestVersion")]
        public string LatestVersion { get; set; }

        #endregion
    }
}
