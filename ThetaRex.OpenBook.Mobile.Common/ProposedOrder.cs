﻿// <copyright file="ProposedOrder.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using System;
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
        /// Gets or sets the unique identifier of the user who created this record.
        /// </summary>
        public int CreatedUserId { get; set; }

        /// <summary>
        /// Gets or sets the time this record was created.
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who last modified this record.
        /// </summary>
        public int ModifiedUserId { get; set; }

        /// <summary>
        /// Gets or sets the time this record was last modified.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

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