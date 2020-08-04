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
        /// Get All Product Definitions.
        /// </summary>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products
        /// </returns>
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            using (StreamReader file = File.OpenText(@"..\FelFeltory.DataAccess\Data\products.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                IEnumerable<Product> products =
                    (IEnumerable<Product>)serializer.Deserialize(file, typeof(IEnumerable<Product>));

                return products;
            }
        }
        /// <summary>
        /// Gets all Batches in the Inventory which still have available
        /// Portions.
        /// </summary>
        /// <param name="freshness">
        /// Freshness of the Batch. If the parameter is not passed then all Batches are returned.
        /// </param>
        /// <returns>
        /// A Task which will resolve into an IEnumerable of Products.
        /// </returns
        public async Task<IEnumerable<Batch>> GetBatches(Freshness? freshness)
        {
            using (StreamReader file = File.OpenText(@"..\FelFeltory.DataAccess\Data\products.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                IEnumerable<Batch> allBatches =
                    (IEnumerable<Batch>)serializer.Deserialize(file, typeof(IEnumerable<Batch>));

                if (freshness == null)
                {
                    return allBatches;
                }

                IEnumerable<Batch> batches = allBatches.Where(b => b.Freshness == freshness);
                return batches;
            }
        }
    }
}
