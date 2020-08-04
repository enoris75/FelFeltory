using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.Models
{
    /// <summary>
    /// Class representing an Inventory Batch.
    /// </summary>
    public class Batch
    {
        /// <summary>
        /// ID of the Batch
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Expiration Date of the Batch (in UTC).
        /// </summary>
        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Quantity of pieces currently available in the Batch.
        /// </summary>
        [JsonProperty("availableQuantity")]
        public int AvailableQuantity { get; set; }

        /// <summary>
        /// Quantity of Portions at Batch creation.
        /// </summary>
        [JsonProperty("batchSize")]
        public int BatchSize { get; set; }

        /// <summary>
        /// Id of the Product of the Batch.
        /// </summary>
        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Get the freshness of the Batch based on the Expiration date and the current date/time.
        /// </summary>
        public Freshness Freshness
        {
            get
            {
                if (DateTime.UtcNow > this.Expiration)
                {
                    return Freshness.Expired;
                }
                else if (DateTime.UtcNow.AddDays(1) > this.Expiration)
                {
                    // Note: Expiring today is calculated as expiring within the next 24 hours,
                    // not necessarily by the end of the current day.
                    return Freshness.ExpiringToday;
                }
                else
                {
                    return Freshness.Fresh;
                }
            }
        }
    }
}
