using Newtonsoft.Json;
using System;

namespace FelFeltory.DataModels
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
        [JsonProperty("size")]
        public int BatchSize { get; set; }

        /// <summary>
        /// Id of the Product of the Batch.
        /// </summary>
        [JsonProperty("pid")]
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

        /// <summary>
        /// Return an instance of Batch based on the passed parameters
        /// </summary>
        /// <param name="productId">ID of the Product the Batch is made of.</param>
        /// <param name="batchSize">Number of Portions in the Batch.</param>
        /// <param name="expirationDate">Expiration Date of the Batch.</param>
        /// <returns>
        /// A new Instance of Batch.
        /// </returns>
        public static Batch GetInstance(
            Guid productId,
            int batchSize,
            DateTime expirationDate
            )
        {
            Batch b = new Batch();
            // Create the new Guid Randomly
            b.Id = Guid.NewGuid();
            b.ProductId = productId;
            b.BatchSize = batchSize;
            b.AvailableQuantity = batchSize;
            b.Expiration = expirationDate;
            return b;
        }

        /// <summary>
        /// Return an instance of Batch based on the passed parameters.
        /// Defines the expiration date/time as in 7 days from now
        /// </summary>
        /// <param name="productId">ID of the Product the Batch is made of.</param>
        /// <param name="batchSize">Number of Portions in the Batch.</param>
        /// <returns>
        /// A new Instance of Batch.
        /// </returns>
        public static Batch GetInstance(
            Guid productId,
            int batchSize
            )
        {
            return GetInstance(productId, batchSize, DateTime.UtcNow.AddDays(7));
        }
    }
}
