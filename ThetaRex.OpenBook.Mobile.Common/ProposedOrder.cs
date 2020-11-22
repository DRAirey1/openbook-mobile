// <copyright file="ProposedOrder.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A proposed order.
    /// </summary>
    public class ProposedOrder
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the order type code.
        /// </summary>
        public OrderTypeCode OrderTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unique proposed order identifier.
        /// </summary>
        public int ProposedOrderId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public int SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the side code.
        /// </summary>
        public SideCode SideCode { get; set; }

        /// <summary>
        /// Gets or sets the time-in-force code.
        /// </summary>
        public TimeInForceCode TimeInForce { get; set; }
    }
}