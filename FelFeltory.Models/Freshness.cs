using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelFeltory.Models
{
    /// <summary>
    /// Class containing the definition of Freshness.
    /// </summary>
    public sealed class Freshness
    {
        /// <summary>
        /// Fresh: more than 1 day of shelf life remaining.
        /// </summary>
        public static readonly string Fresh = "Fresh";
        /// <summary>
        /// Expiring Today.
        /// </summary>
        public static readonly string ExpiringToday = "Expiring Today";
        /// <summary>
        /// Already Expired.
        /// </summary>
        public static readonly string Expired = "Expired";
    }
}
