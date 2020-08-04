using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.RequestModels
{
    /// <summary>
    /// Class mapping the Payload of a "AddBatch" request
    /// </summary>
    public class AddBatchRequestBody
    {
        /// <summary>
        /// Product ID the Batch will consist of.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Number of Portions in the Batch.
        /// </summary>
        public int BatchSize { get; set; }
        /// <summary>
        /// Expiration Date of the Batch.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

    }
}
