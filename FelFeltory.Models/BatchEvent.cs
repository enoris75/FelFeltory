using System;
using Newtonsoft.Json;

namespace FelFeltory.Models
{
    public class BatchEvent
    {
        /// <summary>
        /// ID of the Batch.
        /// </summary>
        [JsonProperty("batchId")]
        public Guid BatchId { get; set; }
        /// <summary>
        /// Date of the event.
        /// </summary>
        [JsonProperty("date")]
        public DateTime EventDate { get; set; }
        /// <summary>
        /// Type of the event.
        /// </summary>
        [JsonProperty("type")]
        public BatchEventType EventType { get; set; }
        /// <summary>
        /// Number of Portions available in the Batch right after the Event.
        /// </summary>
        [JsonProperty("quantity")]
        public int AvailableQuantity { get; set; }
        /// <summary>
        /// Freshness of the Batch at the time of the Event.
        /// </summary>
        [JsonProperty("freshness")]
        public Freshness Freshness { get; set; }
    }
}
