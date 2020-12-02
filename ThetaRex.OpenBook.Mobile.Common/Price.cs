// <copyright file="Price.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    /// <summary>
    /// A price quote.
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Gets or sets the closing price.
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the unique FIGI identifier.
        /// </summary>
        public string Figi { get; set; }

        /// <summary>
        /// Gets or sets the last price.
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique price id.
        /// </summary>
        public int PriceId { get; set; }

        /// <summary>
        /// Gets or sets the unique security id.
        /// </summary>
        public int SecurityId { get; set; }
    }
}