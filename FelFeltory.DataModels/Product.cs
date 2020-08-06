using Newtonsoft.Json;
using System;

namespace FelFeltory.DataModels
{
    /// <summary>
    /// Class representing Products.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier of the product.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the Product.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Description of the Product.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
