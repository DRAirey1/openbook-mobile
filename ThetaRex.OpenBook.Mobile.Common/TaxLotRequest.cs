// <copyright file="TaxLotRequest.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class TaxLotRequest
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public AccountId AccountId { get; set; }

        /// <summary>
        /// Gets or sets the order quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public SecurityId SecurityId { get; set; }
    }
}