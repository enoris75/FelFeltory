using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FelFeltory.Models;
using System.IO;

namespace FelFeltory.DataAccess
{
    public interface IDataAccessService
    {
        /// <summary>
        /// Gets All Product Definitions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        Task<IEnumerable<Product>> GetAllProducts();

        /// <summary>
        /// Gets all Batches in the Inventory which still have available
        /// Portions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        Task<IEnumerable<Batch>> GetBatches(Freshness? freshness);

        /// <summary>
        /// Gets the history of the given Batch.
        /// </summary>
        /// <param name="id">
        /// ID identifying the batch.
        /// </param>
        /// <returns>
        /// A Task which will resolve in to an IEnumerable of Batch Events.
        /// </returns>
        Task<IEnumerable<BatchEvent>> GetBatchHistory(Guid id);
    }
}
