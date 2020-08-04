using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.Models
{
    /// <summary>
    /// Class containing the definition of Batch related event types.
    /// </summary>
    public sealed class BatchEventType
    {
        /// <summary>
        /// The Batch has been received and added to the inventory.
        /// </summary>
        public static readonly string Received = "Batch has been received.";
        /// <summary>
        /// Portions have been removed from the Batch.
        /// </summary>
        public static readonly string PortionsRemoved = "Portions have been removed from Batch.";
        /// <summary>
        /// All the (remaining) Portions have been removed from the Batch.
        /// </summary>
        public static readonly string Emptied = "The last Portions have been removed from the Batch.";
        /// <summary>
        /// The Batch has been disposed of (e.g. because expired).
        /// </summary>
        public static readonly string DisposedOf = "Batch has been disposed of";
    }
}
