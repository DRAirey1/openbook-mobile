// <copyright file="SecurityList.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    /// <summary>
    /// A Security List.
    /// </summary>
    public class SecurityList
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int SecurityListId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the mnemonic.
        /// </summary>
        public string Mnemonic { get; set; }
    }
}