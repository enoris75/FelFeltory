using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FelFeltory.DataModels;
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
        /// Gets all Batches in the Inventory which still have available Portions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        Task<IEnumerable<Batch>> GetAllBatches();

        /// <summary>
        /// Get all Batches in the Inventory which still have available portions and
        /// have the given Freshness.
        /// </summary>
        /// <param name="freshness">Freshness of the batches.</param>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        Task<IEnumerable<Batch>> GetBatches(Freshness freshness);

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

        /// <summary>
        /// Adds a new Batch to the inventory based on the passed parameters.
        /// </summary>
        /// <param name="productId">ID of the Product the Batch is made of.</param>
        /// <param name="batchSize">Number of Portions in the Batch.</param>
        /// <param name="expirationDate">Expiration Date of the Batch.</param>
        /// <returns>
        /// A Task which will resolve in a Batch instance containing the newly added Batch.
        /// </returns>
        Task<Batch> AddBatch(Guid productId, int batchSize, DateTime expirationDate);

        /// <summary>
        /// Removes the given quantity of Portions from the Batch available stock.
        /// </summary>
        /// <param name="batchId">
        /// ID of the Batch the Portions are removed from.
        /// </param>
        /// <param name="quantity">
        /// Quantity of Portions to remove.
        /// </param>
        /// <returns>
        /// A Task which will resolve in a Batch instance with the updated data.
        /// </returns>
        Task<Batch> RemoveFromBatch(Guid batchId, int quantity);

        /// <summary>
        /// Updates the Expiration date of a Batch.
        /// </summary>
        /// <param name="batchId">
        /// ID of the Batch.
        /// </param>
        /// <param name="newExpirationDate">
        /// New Expiration Date.
        /// </param>
        /// <returns>
        /// A Task which resolves in a Batch instance with the updated data.
        /// </returns>
        Task<Batch> FixExpirationDate(Guid batchId, DateTime newExpirationDate);
    }
}
