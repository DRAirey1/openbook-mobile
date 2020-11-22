// <copyright file="ManagedAccount.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class ManagedAccount
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique managed account identifier.
        /// </summary>
        public int ManagedAccountId { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the market value of the account.
        /// </summary>
        public decimal NetAssetValue { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the lot handling code.
        /// </summary>
        public LotHandlingCode LotHandling { get; set; }
    }
}