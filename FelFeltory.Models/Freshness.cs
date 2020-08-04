using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.Models
{
    /// <summary>
    /// Enum containing the definition of Freshness.
    /// </summary>
    public enum Freshness
    {
        /// <summary>
        /// Fresh: more than 1 day of shelf life remaining.
        /// </summary>
        Fresh,
        /// <summary>
        /// Expiring Today.
        /// </summary>
        ExpiringToday,
        /// <summary>
        /// Already Expired.
        /// </summary>
        Expired
    }
}
