// <copyright file="Security.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A security.
    /// </summary>
#pragma warning disable CA1724
    public class Security
    {
#pragma warning restore CA1724
        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the unique FIGI identifier.
        /// </summary>
        public string Figi { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the security list id.
        /// </summary>
        public int SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the unique external identifier.
        /// </summary>
        public SecurityTypeCode SecurityTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the ticker symbol.
        /// </summary>
        public string Ticker { get; set; }
    }
}