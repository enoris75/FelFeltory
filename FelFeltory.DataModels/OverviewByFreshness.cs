using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FelFeltory.DataModels
{
    /// <summary>
    /// Class representing the overview of the Inventory on a Freshness base.
    /// </summary>
    public class OverviewByFreshness
    {
        /// <summary>
        /// Dictionary of the available Batches by freshness.
        /// </summary>
        private Dictionary<Freshness, int> BatchesByFreshness;
        /// <summary>
        /// Dictionary of the available Portions by freshness.
        /// </summary>
        private Dictionary<Freshness, int> PortionsByFreshness;

        /// <summary>
        /// Fresh Batches
        /// </summary>
        [JsonProperty("FreshBatches")]
        public int FreshBatches
        {
            get
            {
                return BatchesByFreshness[Freshness.Fresh];
            }
        }
        /// <summary>
        /// Fresh Portions
        /// </summary>
        [JsonProperty("FreshPortions")]
        public int FreshPortions
        {
            get
            {
                return PortionsByFreshness[Freshness.Fresh];
            }
        }
        /// <summary>
        /// Expired Batches
        /// </summary>
        [JsonProperty("ExpiredBatches")]
        public int ExpiredBatches
        {
            get
            {
                return BatchesByFreshness[Freshness.Expired];
            }
        }
        /// <summary>
        /// Expired Portions
        /// </summary>
        [JsonProperty("ExpiredPortions")]
        public int ExpiredPortions
        {
            get
            {
                return PortionsByFreshness[Freshness.Expired];
            }
        }
        /// <summary>
        ///  Expiring Today Batches
        /// </summary>
        [JsonProperty("ExpiringTodayBatches")]
        public int ExpiringTodayBatches
        {
            get
            {
                return BatchesByFreshness[Freshness.ExpiringToday];
            }
        }
        /// <summary>
        /// Expiring Today Portions
        /// </summary>
        [JsonProperty("ExpiringTodayPortions")]
        public int ExpiringTodayPortions
        {
            get
            {
                return PortionsByFreshness[Freshness.ExpiringToday];
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OverviewByFreshness()
        {
            BatchesByFreshness = new Dictionary<Freshness, int>();
            PortionsByFreshness = new Dictionary<Freshness, int>();

            // Initialize the dictionary with zeroes in all entries.

            // Note for the reviewer: this shows the limit of using an Enum to define
            // the possible values Freshness can have.
            foreach (Freshness f in (Freshness[])Enum.GetValues(typeof(Freshness)))
            {
                BatchesByFreshness.Add(f, 0);
                PortionsByFreshness.Add(f, 0);
            }
        }

        /// <summary>
        /// Add the content of the given batch to the corresponding Dictionary entries.
        /// </summary>
        /// <param name="batch">
        /// Batch to be added.
        /// </param>
        public void AddBatchesToOverview(List<Batch> batches)
        {
            batches.ForEach(
                batch =>
                {
                    Freshness f = batch.Freshness;
                    // Add one Batch to the proper category
                    BatchesByFreshness[f] += 1;
                    // Add the available portions to the proper category
                    PortionsByFreshness[f] += batch.AvailableQuantity;
                });
        }
    }
}
