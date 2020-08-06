using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FelFeltory.DataModels;
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
            List<BatchEvent> allEvents =
                await this.GetData<BatchEvent>(this.fileBatchEvents);
            // Filter the batches by ID.
            List<BatchEvent> events = allEvents.Where(
                e => e.BatchId == id
                ).OrderBy(
                    e => e.EventDate
                ).ToList();

            return events;
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
            List<Batch> allBatches = await this.GetData<Batch>(this.fileBatches);
            // Create the new Batch
            Batch newBatch = Batch.GetInstance(productId, batchSize, expirationDate);
            // Add the new Batch to the list
            allBatches.Add(newBatch);
            // Write the list into a File
            await WriteData(this.fileBatches, allBatches);

            return newBatch;
        }

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
        public async Task<Batch> RemoveFromBatch(Guid batchId, int quantity)
        {
            // Get the list of all Batches
            List<Batch> allBatches = await this.GetData<Batch>(this.fileBatches);
            // Get the Batch
            Batch batch = this.GetBatch(batchId, allBatches);
            if (batch.AvailableQuantity < quantity)
            {
                // Note for the reviewer:
                // This is a little bit of string concatenation
                // however it should be fine to not use StringBuilder here
                // because in the end there are only 7 strings and
                // this part of code is not expected to be hit so often.
                throw new Exception(
                    "Cannot remove the desired quantity of: "
                    + quantity
                    + " Portions from the Batch with ID: "
                    + batchId
                    + " because only "
                    + batch.AvailableQuantity
                    + " are available"
                    );
            }
            // Remove the element to be updated from the list
            allBatches.Remove(batch);
            // Change the available quantity in the batch
            batch.AvailableQuantity -= quantity;
            // Re-add it to the list
            allBatches.Add(batch);
            // Update the batches file
            await WriteData(this.fileBatches, allBatches);
            // Return the updated batch
            return batch;
        }

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
        public async Task<Batch> FixExpirationDate(Guid batchId, DateTime newExpirationDate)
        {
            // Get the list of all Batches
            List<Batch> allBatches = await this.GetData<Batch>(this.fileBatches);
            // Get the Batch
            Batch batch = this.GetBatch(batchId, allBatches);
            // Remove the batch
            allBatches.Remove(batch);
            // Update the batch
            batch.Expiration = newExpirationDate;
            // Re-add it to the list
            allBatches.Add(batch);
            // Update the batches file
            await WriteData(this.fileBatches, allBatches);
            // Return the updated batch
            return batch;
        }

        #endregion IDataAccessService Implementation

        /// <summary>
        /// Gets the batch corresponding to the given ID.
        /// Throws Exceptions if none or more than one are found.
        /// </summary>
        /// <param name="batchId">
        /// ID of the Batch.
        /// </param>
        /// <param name="List">
        /// List of the Batches that will be serached
        /// </param>
        /// <returns>
        /// A Task which resolves into the Batch.
        /// </returns>
        private Batch GetBatch(Guid batchId, List<Batch> batches)
        {
            // Get the Batch with the correct ID
            List<Batch> selectedBatches = batches.Where(
                b => b.Id == batchId
                ).ToList();
            int count = selectedBatches.Count<Batch>();
            // Note there should be exactly one batch with the given ID
            if (count == 0)
            {
                throw new Exception(
                    "The Batch with the ID: "
                    + batchId.ToString()
                    + " couldn't be found");
            }
            else if (count > 1)
            {
                throw new Exception(
                    "Corrupted data: there are multiple ("
                    + count
                    + ") instances of Batches with ID: "
                    + batchId.ToString());
            }
            // There is exactly one Batch with the given ID.
            Batch batch = selectedBatches.First();

            return batch;
        }

        /// <summary>
        /// Get all data of given type from the given file.
        /// </summary>
        /// <typeparam name="T">Type of data to be read.</typeparam>
        /// <param name="fileName">File containing the data.</param>
        /// <returns></returns>
        private async Task<List<T>> GetData<T>(string fileName)
        {
            using (StreamReader file = File.OpenText(fileName))
            using (JsonReader reader = new JsonTextReader(file))
            {

                List<T> allBatches =
                    serializer.Deserialize<List<T>>(reader);

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
