﻿using Newtonsoft.Json;

namespace EdgeOS.API.Core.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class NumberOfRoutesResponse : IResponse
    {
        /// <summary>The object containing counts of the EdgeOS network routes.</summary>
        [JsonProperty(PropertyName = "num-routes")]
        public NumberOfRoutes NumberOfRoutes;
    }
}