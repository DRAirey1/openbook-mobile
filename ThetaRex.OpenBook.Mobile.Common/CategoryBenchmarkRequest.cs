// <copyright file="CategoryBenchmarkRequest.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class CategoryBenchmarkRequest
    {
        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets benchmark unique identifier.
        /// </summary>
        public BenchmarkId BenchmarkId { get; set; }

        /// <summary>
        /// Gets or sets classification unique identifier.
        /// </summary>
        public CategoryId CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the weight of the category.
        /// </summary>
        public decimal Weight { get; set; }
    }
}