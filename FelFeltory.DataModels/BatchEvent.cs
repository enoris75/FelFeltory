using System;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace FelFeltory.DataModels
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

        /// <summary>
        /// Returns a BatchEvent Instance.
        /// </summary>
        /// <param name="batch">
        /// Batch that has been added.
        /// </param>
        /// <param name="type">
        /// Type of the event.
        /// </param>
        /// <returns>
        /// A BatchEvent instance.
        /// </returns>
        public static BatchEvent GetInstance(Batch batch, BatchEventType type)
        {
            BatchEvent e = new BatchEvent();
            e.EventType = type;
            e.BatchId = batch.Id;
            e.EventDate = DateTime.UtcNow;
            e.AvailableQuantity = batch.AvailableQuantity;
            e.Freshness = batch.Freshness;
            return e;
        }
    }
}
