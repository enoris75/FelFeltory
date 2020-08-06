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
    public sealed class DataAccessService : IDataAccessService
    {
        /// <summary>
        /// Instance of Data Handler used to read and write data.
        /// </summary>
        private readonly IDataHandler DataHandler;

        public DataAccessService(IDataHandler dataHandler)
        {
            DataHandler = dataHandler;
        }

        #region IDataAccessService Implementation

        /// <summary>
        /// Get All Product Definitions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products
        /// </returns>
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await DataHandler.GetData<Product>(DataSource.Products);
        }

        /// <summary>
        /// Gets all Batches in the Inventory which still have available Portions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns>
        public async Task<IEnumerable<Batch>> GetAllBatches()
        {
            return await DataHandler.GetData<Batch>(DataSource.Batches);
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
            IEnumerable<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);

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
                await DataHandler.GetData<BatchEvent>(DataSource.BatchEvents);
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
            List<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);
            // Create the new Batch
            Batch newBatch = Batch.GetInstance(productId, batchSize, expirationDate);
            // Add the new Batch to the list
            allBatches.Add(newBatch);
            // Write the list into a File
            await DataHandler.WriteData(DataSource.Batches, allBatches);
            // Create an event corresponding to this action
            BatchEvent e = BatchEvent.GetInstance(newBatch, BatchEventType.Added);
            // Add the event to the history
            await AddBatchEventToHistory(e);
            // return the newly added batch
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
            List<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);
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
            await DataHandler.WriteData(DataSource.Batches, allBatches);
            // Create an event corresponding to this action
            BatchEvent e;
            if (batch.AvailableQuantity > 0)
            {
                e = BatchEvent.GetInstance(batch, BatchEventType.PortionsRemoved);
            }
            else
            {
                // the batch is now empty
                e = BatchEvent.GetInstance(batch, BatchEventType.Emptied);
            }
            // Add the event to the history
            await AddBatchEventToHistory(e);
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
            List<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);
            // Get the Batch
            Batch batch = this.GetBatch(batchId, allBatches);
            // Remove the batch
            allBatches.Remove(batch);
            // Update the batch
            batch.Expiration = newExpirationDate;
            // Re-add it to the list
            allBatches.Add(batch);
            // Update the batches file
            await DataHandler.WriteData(DataSource.Batches, allBatches);
            // Return the updated batch
            return batch;
        }

        /// <summary>
        /// Updates the Batch size and available quantity.
        /// </summary>
        /// <param name="batchId">
        /// ID of the Batch.
        /// </param>
        /// <param name="newBatchSize">
        /// Updated size of the Batch.
        /// </param>
        /// <param name="newAvailableQuantity">
        /// Updated available quantity (number of Portions) of the Batch.
        /// </param>
        /// <returns>
        /// A Task which resolves in a Batch instance with the updated data.
        /// </returns>
        public async Task<Batch> FixQuantities(Guid batchId, int newBatchSize, int newAvailableQuantity)
        {
            // Get the list of all Batches
            List<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);
            // Get the Batch
            Batch batch = this.GetBatch(batchId, allBatches);
            // Remove the batch
            allBatches.Remove(batch);
            // Update the batch
            batch.BatchSize = newBatchSize;
            batch.AvailableQuantity = newAvailableQuantity;
            // Re-add it to the list
            allBatches.Add(batch);
            // Update the batches file
            await DataHandler.WriteData(DataSource.Batches, allBatches);
            // Return the updated batch
            return batch;
        }

        /// <summary>
        /// Get the overview of the inventory broke down by the Freshness of the Batches/Portions.
        /// </summary>
        /// <returns>
        /// A Task which resolves in the Overview
        /// </returns>
        public async Task<OverviewByFreshness> GetOverviewByFreshness()
        {
            OverviewByFreshness overview = new OverviewByFreshness();
            // Get the list of all Batches
            List<Batch> allBatches = await DataHandler.GetData<Batch>(DataSource.Batches);
            // Add the batches to the overview
            overview.AddBatchesToOverview(allBatches);
            // return the overview
            return overview;
        }

        #endregion IDataAccessService Implementation

        #region Private Methods
        /// <summary>
        /// Gets the batch corresponding to the given ID.
        /// Throws Exceptions if none or more than one are found.
        /// </summary>
        /// <param name="batchId">
        /// ID of the Batch.
        /// </param>
        /// <param name="List">
        /// List of the Batches that will be searched
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
        /// Add the given BatchEvent to the list of all batch events.
        /// </summary>
        /// <param name="e">
        /// Batch Event to be added.
        /// </param>
        /// <returns>
        /// A Tasks which resovles when the operation completes.
        /// </returns>
        private async Task AddBatchEventToHistory(BatchEvent e)
        {
            // Get all Batches
            List<BatchEvent> allEvents =
                await DataHandler.GetData<BatchEvent>(DataSource.BatchEvents);
            // Add the new event
            allEvents.Add(e);
            // Write the list of events
            await DataHandler.WriteData(DataSource.BatchEvents, allEvents);
        }
        #endregion
    }
}
