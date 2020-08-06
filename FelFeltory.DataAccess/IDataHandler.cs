using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FelFeltory.DataModels;
using System.IO;

namespace FelFeltory.DataAccess
{
    public interface IDataHandler
    {
        /// <summary>
        /// Get all data of given type from the given file.
        /// </summary>
        /// <typeparam name="T">Type of data to be read.</typeparam>
        /// <param name="source">Data Source</param>
        /// <returns>
        /// A Task which results in the List containing the requested data.
        /// </returns>
        Task<List<T>> GetData<T>(DataSource source);

        /// <summary>
        /// Serialize and write the given data to the given file.
        /// </summary>
        /// <typeparam name="T">Type of the data to be serialized.</typeparam>
        /// <param name="source">Data Source</param>
        /// <param name="data">Data to be written</param>
        /// <returns>
        /// A Task which resolves when the operation is complete.
        /// </returns>
        Task WriteData<T>(DataSource source, T data);
    }
}
