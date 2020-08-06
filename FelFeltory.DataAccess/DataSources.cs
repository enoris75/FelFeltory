using System;
using System.Collections.Generic;
using System.Text;

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
}
