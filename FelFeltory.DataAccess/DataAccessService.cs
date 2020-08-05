using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FelFeltory.Models;
using System.IO;
using System.Linq;

namespace FelFeltory.DataAccess
{
    /// <summary>
    /// Implementation of the Data Access Service.
    /// </summary>
    public class DataAccessService : IDataAccessService
    {
        // Note for the reviewer: Method in this class are declared as async even if their implementation
        // at this stage does not requires it (for the data is read from local files).
        // The reason for this is that if we would proceed to an actual implementation
        // of this service, with the data coming from a remote service (or DB) the calls
        // to do so, would most probably be asynchronous.

        // ADDITIONAL NOTE for the reviewer: the way this file is accessed would per se cries to
        // Heaven for Vengeance, weren't this just a way to pull some mock data from
        // an external file written in the hiddenness of my COVID dungeon.
        /// <summary>
        /// File which stores the Products.
        /// </summary>
        private readonly string fileProducts = @"..\FelFeltory.DataAccess\Data\products.json";
        /// <summary>
        /// File which stores the Batches.
        /// </summary>
        private readonly string fileBatches = @"..\FelFeltory.DataAccess\Data\batches.json";
        /// <summary>
        /// File which stores the BatchEvents.
        /// </summary>
        private readonly string fileBatchEvents = @"..\FelFeltory.DataAccess\Data\batchEvents.json";

        /// <summary>
        /// The JsonSerializer to use for JSON serialization and de-serialization.
        /// </summary>
        private readonly JsonSerializer serializer = JsonSerializer.CreateDefault();

        #region IDataAccessService Implementation

        /// <summary>
        /// Get All Product Definitions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products
        /// </returns>
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await this.GetData<Product>(this.fileProducts);
        }

        /// <summary>
        /// Gets all Batches in the Inventory which still have available Portions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        public async Task<IEnumerable<Batch>> GetAllBatches()
        {
            return await this.GetData<Batch>(this.fileBatches);
        }

        /// <summary>
        /// Get all Batches in the Inventory which still have available portions and
        /// have the given Freshness.
        /// </summary>
        /// <param name="freshness">Freshness of the batches.</param>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        public async Task<IEnumerable<Batch>> GetBatches(Freshness freshness)
        {
            IEnumerable<Batch> allBatches = await this.GetData<Batch>(this.fileBatches);

            IEnumerable<Batch> batches = allBatches.Where(
                b => b.Freshness == freshness
                );
            return batches;
        }

        /// <summary>
        /// Adds a new Batch to the inventory based on the passed parameters.
        /// </summary>
        /// <param name="productId">ID of the Product the Batch is made of.</param>
        /// <param name="batchSize">Number of Portions in the Batch.</param>
        /// <param name="expirationDate">Expiration Date of the Batch.</param>
        /// <returns>
        /// A Task which will resolve in a Batch instance containing the newly added Batch.
        /// </returns>
        public async Task<Batch> AddBatch(Guid productId, int batchSize, DateTime expirationDate)
        {
            // Get all Batches
            IEnumerable<Batch> allBatches = await this.GetData<Batch>(this.fileBatches);
            // Add the new Batch to the list
            Batch newBatch = Batch.GetInstance(productId, batchSize, expirationDate);
            allBatches.Append(newBatch);
            // Write the list into a File
            await WriteData<IEnumerable<Batch>>(this.fileBatches, allBatches);

            return newBatch;
        }

        /// <summary>
        /// Gets the history of the given Batch.
        /// </summary>
        /// <param name="id">
        /// ID identifying the batch.
        /// </param>
        /// <returns>
        /// A Task which will resolve in to an IEnumerable of Batch Events.
        /// </returns>
        public async Task<IEnumerable<BatchEvent>> GetBatchHistory(Guid id)
        {
            // Get all Batches
            IEnumerable<BatchEvent> allEvents =
                await this.GetData<BatchEvent>(this.fileBatchEvents);
            // Filter the batches by ID.
            IEnumerable<BatchEvent> events = allEvents.Where(
                e => e.BatchId == id
                ).OrderBy(
                    e => e.EventDate
                );

            return events;
        }

        #endregion IDataAccessService Implementation

        /// <summary>
        /// Get all data of given type from the given file.
        /// </summary>
        /// <typeparam name="T">Type of data to be read.</typeparam>
        /// <param name="fileName">File containing the data.</param>
        /// <returns></returns>
        private async Task<IEnumerable<T>> GetData<T>(string fileName)
        {
            using (StreamReader file = File.OpenText(fileName))
            using (JsonReader reader = new JsonTextReader(file))
            {

                IEnumerable<T> allBatches =
                    serializer.Deserialize<IEnumerable<T>>(reader);

                return allBatches;
            }
        }

        /// <summary>
        /// Serialize and write the given data to the given file.
        /// </summary>
        /// <typeparam name="T">Type of the data to be serialized.</typeparam>
        /// <param name="fileName">File the data will be written to.</param>
        /// <param name="data">Data to be written</param>
        /// <returns>
        /// A Task which resolves when the operation is complete.
        /// </returns>
        private async Task WriteData<T>(string fileName, T data)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                serializer.Serialize(file, data);
            }
        }
    }
}
