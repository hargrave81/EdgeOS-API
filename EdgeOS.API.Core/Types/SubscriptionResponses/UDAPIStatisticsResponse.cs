﻿using Newtonsoft.Json;

namespace EdgeOS.API.Core.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class UDAPIStatisticsResponse : IResponse
    {
        /// <summary>The object containing device API statistics, potentially used by UNMS.</summary>
        [JsonProperty(PropertyName = "udapi-statistics")]
        public UDAPIStatistics[] UDAPIStatistics;
    }
}