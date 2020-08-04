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
        /// Expiration Date of the Batch.
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
        /// Freshness of the Batch.
        /// </summary>
        public Freshness Freshness { get; set; }
    }
}
