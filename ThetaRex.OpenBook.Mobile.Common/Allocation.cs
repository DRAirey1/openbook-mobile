// <copyright file="Allocation.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class Allocation
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int AllocationId { get; set; }

        /// <summary>
        /// Gets or sets the order quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public int SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the unique working order identifier.
        /// </summary>
        public int SourceOrderId { get; set; }
    }
}