// <copyright file="AllocationRequest.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using System;
    using Newtonsoft.Json;
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class AllocationRequest
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public AccountId AccountId { get; set; }

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
        /// Gets or sets the order quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier for settlement.
        /// </summary>
        public SecurityId SettlementId { get; set; }

        /// <summary>
        /// Gets or sets the side code.
        /// </summary>
        public SideCode SideCode { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public SourceOrderId SourceOrderId { get; set; }
    }
}