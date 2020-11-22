// <copyright file="WorkingOrderRequest.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class WorkingOrderRequest
    {
        /// <summary>
        /// Gets or sets the unique blotter identifier.
        /// </summary>
        public BlotterId BlotterId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created this record.
        /// </summary>
        public int CreatedUserId { get; set; }

        /// <summary>
        /// Gets or sets the time this record was created.
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public string ExternalId { get; set; }

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
        /// Gets or sets the unique security identifier.
        /// </summary>
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the side code.
        /// </summary>
        public SideCode SideCode { get; set; }

        /// <summary>
        /// Gets the collection of source orders.
        /// </summary>
        public List<SourceOrderRequest> SourceOrders { get; } = new List<SourceOrderRequest>();

        /// <summary>
        /// Gets or sets the time in force.
        /// </summary>
        public TimeInForceCode TimeInForce { get; set; }
    }
}