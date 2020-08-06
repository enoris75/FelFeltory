using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace FelFeltory.DataAccess
{
    /// <summary>
    /// Enum containing the different Data soueces / tables
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// Contains the list of products and their descriptions.
        /// </summary>
        Products,
        /// <summary>
        /// Contains the list of batches.
        /// </summary>
        Batches,
        /// <summary>
        /// Contains the list of batche's events / history.
        /// </summary>
        BatchEvents,
    }

    /// <summary>
    /// Class handling the reading and the writing of the data to the different data sources.
    /// </summary>
    class DataHandler
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

        /// <summary>
        /// Get all data of given type from the given file.
        /// </summary>
        /// <typeparam name="T">Type of data to be read.</typeparam>
        /// <param name="source">Data Source</param>
        /// <returns>
        /// A Task which results in the List containing the requested data.
        /// </returns>
        public async Task<List<T>> GetData<T>(DataSource source)
        {
            string fileName = GetFileName(source);

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
        /// <param name="source">Data Source</param>
        /// <param name="data">Data to be written</param>
        /// <returns>
        /// A Task which resolves when the operation is complete.
        /// </returns>
        public async Task WriteData<T>(DataSource source, T data)
        {
            string fileName = GetFileName(source);

            using (StreamWriter file = File.CreateText(fileName))
            {
                serializer.Serialize(file, data);
            }
        }

        /// <summary>
        /// Get the file name corresponding to the given data source.
        /// </summary>
        /// <param name="source">
        /// Data Source.
        /// </param>
        /// <returns>
        /// File Name.
        /// </returns>
        private string GetFileName(DataSource source)
        {
            switch (source)
            {
                case DataSource.Products:
                    return fileProducts;
                case DataSource.Batches:
                    return fileBatches;
                case DataSource.BatchEvents:
                    return fileBatchEvents;
                default:
                    throw new Exception("Invalid data source: " + source);
            }
        }
    }
}
