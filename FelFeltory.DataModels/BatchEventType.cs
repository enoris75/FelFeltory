using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.DataModels
{
    /// <summary>
    /// Enum containing the definition of Batch related event types.
    /// </summary>
    public enum BatchEventType
    {
        /// <summary>
        /// The Batch has been received and added to the inventory.
        /// </summary>
        Added,
        /// <summary>
        /// Portions have been removed from the Batch.
        /// </summary>
        PortionsRemoved,
        /// <summary>
        /// All the (remaining) Portions have been removed from the Batch.
        /// </summary>
        Emptied,
        /// <summary>
        /// The Batch has been disposed of (e.g. because expired).
        /// </summary>
        DisposedOf
    }
}
