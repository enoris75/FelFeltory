using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FelFeltory.Models;
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
        /// An IEnumerable containing all the Batches in the Inventory.
        /// </returns>
        [HttpGet]
        [Route("AllBatches")]
        public async Task<ActionResult<IEnumerable<Batch>>> GetAllBatches()
        {
            IEnumerable<Product> products = await this.AccessService.GetAllProducts();

            List<Batch> batchList = new List<Batch>();
            Batch b1 = new Batch();
            b1.Expiration = new DateTime();
            b1.ProductId = new Guid();
            b1.AvailableQuantity = 999;
            b1.BatchSize = 1000;
            batchList.Add(b1);

            return Ok(batchList);
        }

        /// <summary>
        /// Retrieve all the Batches having the given freshness.
        /// </summary>
        /// <param name="freshness">Freshness of the Batch (e.g. Fresh, Expiring, Expired)</param>
        /// <returns>An IEnumerable of Batches having the requested Freshness.</returns>
        [HttpGet]
        [Route("BatchesByFreshness")]
        public async Task<ActionResult<IEnumerable<Batch>>> GetBatchesByFreshness([FromRoute] Freshness freshness)
        {
            return Ok("Not implemented yet");
        }

        /// <summary>
        /// Retrieve the history of the given Batch
        /// </summary>
        /// <param name="id">ID of the Batch</param>
        /// <returns>An IEnumerable of Events related to the given Batch history.</returns>
        [HttpGet]
        [Route("BatchHistory")]
        public async Task<ActionResult<IEnumerable<BatchEvent>>> GetBatchHistory(
            [FromRoute] Guid id
            )
        {
            return Ok("Not implemented yet");
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
        public async Task<IActionResult> AddBatch(
            [FromBody] AddBatchRequestBody requestBody
            )
        {
            return Ok("Not implemented yet");
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
        [Route("RemoveFromBatch")]
        public async Task<IActionResult> RemoveFromBatch(
            [FromRoute] Guid id,
            [FromBody] int quantity
            )
        {
            return Ok("Not implemented yet");
        }

        /// <summary>
        /// Fixes a wrongly set Expiration Date (in case some mistake was made).
        /// </summary>
        /// <param name="id">ID of the Batch the Expiration Date needs to be fixed.</param>
        /// <param name="newExpirationDate">New Expiration Date</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("FixExpirationDate")]
        public async Task<IActionResult> FixExpirationDate(
            [FromRoute] Guid id,
            [FromBody] DateTime newExpirationDate
            )
        {
            return Ok("Not implemented yet");
        }
    }
}
