using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FelFeltory.DataModels;
using FelFeltory.RequestModels;
using FelFeltory.DataAccess;

namespace FelFeltory.Controllers
{
    /// <summary>
    /// Controller for the API Interacting with FelFel inventory system.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        /// <summary>
        /// The ILogger to use.
        /// </summary>
        private readonly ILogger<InventoryController> Logger;
        /// <summary>
        /// The accessService to use to get and set the data.
        /// </summary>
        private readonly IDataAccessService AccessService;

        public InventoryController(
            ILogger<InventoryController> logger,
            IDataAccessService accessService
            )
        {
            this.Logger = logger;
            this.AccessService = accessService;
        }

        /// <summary>
        /// Return the list of all the Batches in the inventory,
        /// regardless of their status, etc.
        /// </summary>
        /// <returns>
        /// A Task which resolves in an ActionResult containing
        /// an IEnumerable containing all the Batches in the Inventory.
        /// </returns>
        [HttpGet]
        [Route("AllBatches")]
        public async Task<ActionResult> GetAllBatches()
        {
            IEnumerable<Batch> batchList = await AccessService.GetAllBatches();

            return Ok(batchList);
        }

        /// <summary>
        /// Retrieve all the Batches having the given freshness.
        /// </summary>
        /// <param name="freshness">Freshness of the Batch (e.g. Fresh, Expiring, Expired)</param>
        /// <returns>
        /// A Task which resolves in an ActionResult containing 
        /// an IEnumerable of Batches having the requested Freshness.
        /// </returns>
        [HttpGet]
        [Route("BatchesByFreshness/{freshness}")]
        public async Task<ActionResult> GetBatchesByFreshness([FromRoute] Freshness freshness)
        {
            IEnumerable<Batch> batches = await AccessService.GetBatches(freshness);

            return Ok(batches);
        }

        /// <summary>
        /// Retrieve the history of the given Batch
        /// </summary>
        /// <param name="batchId">ID of the Batch</param>
        /// <returns>An IEnumerable of Events related to the given Batch history.</returns>
        [HttpGet]
        [Route("BatchHistory/{batchId}/")]
        public async Task<ActionResult> GetBatchHistory(
            [FromRoute] Guid batchId
            )
        {
            IEnumerable<BatchEvent> events = await AccessService.GetBatchHistory(batchId);

            return Ok(events);
        }

        /// <summary>
        /// Adds the Batch into the Inventory.
        /// </summary>
        /// <param name="productId">ID of the Product the Batch consists of.</param>
        /// <param name="batchSize">Number of Portions in the Batch.</param>
        /// <param name="expiration">Expiration Date of the Batch.</param>
        /// <returns>
        /// A Task which results in an IActionResult describing the outcome of the operation.
        /// </returns>
        [HttpPut]
        [Route("AddBatch")]
        public async Task<ActionResult> AddBatch(
            [FromBody] AddBatchRequestBody requestBody
            )
        {
            Batch newBatch = await AccessService.AddBatch(
                requestBody.ProductId,
                requestBody.BatchSize,
                requestBody.ExpirationDate
                );
            return Ok(newBatch);
        }

        /// <summary>
        /// Removes the given number of Portions from the given Batch.
        /// </summary>
        /// <param name="id">ID of the Batch the Portions are removed from.</param>
        /// <param name="quantity">Number of Portions to be removed.</param>
        /// <returns>
        /// A Task which results in an IActionResult describing the outcome of the operation.
        /// </returns>
        [HttpPost]
        [Route("RemoveFromBatch/{batchId}/{quantity}/")]
        public async Task<ActionResult> RemoveFromBatch(
            [FromRoute] Guid batchId,
            [FromRoute] int quantity
            )
        {
            if (quantity < 0)
            {
                return BadRequest(new
                {
                    error = "invalid quantity",
                    description = "the quantity cannot be negative"
                });
            }

            Batch updatedBatch = await AccessService.RemoveFromBatch(batchId, quantity);

            return Ok(updatedBatch);
        }

        /// <summary>
        /// Gets the breakdown by freshness of the Inventory.
        /// </summary>
        /// <returns>
        /// A Task which resolves in an ActionResult containing the Overview.
        /// </returns>
        [HttpGet]
        [Route("OverviewByFreshness")]
        public async Task<ActionResult> GetOverviewByFreshness()
        {
            OverviewByFreshness overview = await AccessService.GetOverviewByFreshness();
            return Ok(overview);
        }

        /// <summary>
        /// Fixes a wrongly set Expiration Date (in case some mistake was made).
        /// </summary>
        /// <param name="id">ID of the Batch the Expiration Date needs to be fixed.</param>
        /// <param name="newExpirationDate">New Expiration Date</param>
        /// <returns>
        /// A Task which resolves in an ActionResult containing the updated Batch.
        /// </returns>
        [HttpPatch]
        [Route("FixExpirationDate/{batchId}/{newExpirationDate}/")]
        public async Task<ActionResult> FixExpirationDate(
            [FromRoute] Guid batchId,
            [FromRoute] DateTime newExpirationDate
            )
        {
            Batch batch = await AccessService.FixExpirationDate(
                    batchId,
                    newExpirationDate
                );

            return Ok(batch);
        }

        /// <summary>
        /// Fixes wrongly set Batch size and available quantity.
        /// </summary>
        /// <param name="batchId">
        /// ID of the batch
        /// </param>
        /// <param name="newBatchSize">
        /// Updated Batch size.
        /// </param>
        /// <param name="newAvailableQuantity">
        /// Updated available quantity (number of Portions)
        /// </param>
        /// <returns>
        /// A Task which resolves in an ActionResult containing the updated Batch.
        /// </returns>
        [HttpPatch]
        [Route("FixExpirationDate/{batchId}/{newBatchSize}/{newAvailableQuantity}")]
        public async Task<ActionResult> FixQuantities(
            [FromRoute] Guid batchId,
            [FromRoute] int newBatchSize,
            [FromRoute] int newAvailableQuantity
            )
        {
            Batch batch = await AccessService.FixQuantities(
                    batchId,
                    newBatchSize,
                    newAvailableQuantity
                );

            return Ok(batch);
        }
    }
}
