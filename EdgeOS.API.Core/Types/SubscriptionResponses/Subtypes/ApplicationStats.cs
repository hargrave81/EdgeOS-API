using Newtonsoft.Json;

namespace EdgeOS.API.Core.Types
{
    /// <summary>An object that contains traffic statistics for a certain observed application type via Deep Packet Inspection (DPI).</summary>
    public class ApplicationStats
    {
        /// <summary>The number of bytes transmitted from this observed application.</summary>
        [JsonProperty(PropertyName = "tx_bytes")]
        public uint tx_bytes;

        /// <summary>The rate at which bytes are currently being transmitted from this observed application.</summary>
        [JsonProperty(PropertyName = "tx_rate")]
        public uint tx_rate;

        /// <summary>The number of bytes received by this observed application.</summary>
        [JsonProperty(PropertyName = "rx_bytes")]
        public uint rx_bytes;

        /// <summary>The rate at which bytes are currently being received by this observed application.</summary>
        [JsonProperty(PropertyName = "rx_rate")]
        public uint rx_rate;
    }
}