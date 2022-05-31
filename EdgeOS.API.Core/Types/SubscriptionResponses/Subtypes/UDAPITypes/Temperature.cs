﻿using Newtonsoft.Json;

namespace EdgeOS.API.Core.Types.SubscriptionResponses.Subtypes.UDAPITypes
{
    /// <summary>An object that contains a temperature reading.</summary>
    public class Temperature
    {
        /// <summary>Unknown.</summary>
        [JsonProperty(PropertyName = "&ubnt_arr_type;")]
        public string ArrayType;
    }
}