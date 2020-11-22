// <copyright file="CategoryBenchmark.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class CategoryBenchmark
    {
        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets benchmark unique identifier.
        /// </summary>
        public int BenchmarkId { get; set; }

        /// <summary>
        /// Gets or sets classification unique identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets category benchmark unique identifier.
        /// </summary>
        public int CategoryBenchmarkId { get; set; }

        /// <summary>
        /// Gets or sets the weight of the category.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }
    }
}