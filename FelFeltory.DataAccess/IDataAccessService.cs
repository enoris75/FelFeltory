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
        Task<IEnumerable<Batch>> GetAllBatches();
    }
}
