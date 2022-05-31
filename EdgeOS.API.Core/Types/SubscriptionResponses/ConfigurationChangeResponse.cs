﻿using Newtonsoft.Json;

namespace EdgeOS.API.Core.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class ConfigurationChangeResponse : IResponse
    {
        /// <summary>The object that contains status information when EdgeOS is processing a configuration change.</summary>
        [JsonProperty(PropertyName = "config-change")]
        public ConfigurationChange ConfigurationChange;
    }
}