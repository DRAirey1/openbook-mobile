// <copyright file="Blotter.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class Blotter
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int BlotterId { get; set; }

        /// <summary>
        /// Gets or sets the order quantity.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the mnemonic.
        /// </summary>
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }
    }
}