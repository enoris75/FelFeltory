using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.Models
{
    public class BatchEvent
    {
        /// <summary>
        /// ID of the Batch.
        /// </summary>
        public Guid BatchId { get; set; }
        /// <summary>
        /// Date of the event.
        /// </summary>
        public DateTime EventDate { get; set; }
        /// <summary>
        /// Type of the event.
        /// </summary>
        public BatchEventType EventType { get; set; }
        /// <summary>
        /// Number of Portions available in the Batch right after the Event.
        /// </summary>
        public int AvailableQuantity { get; set; }
        /// <summary>
        /// Freshness of the Batch at the time of the Event.
        /// </summary>
        public Freshness Freshness { get; set; }
    }
}
