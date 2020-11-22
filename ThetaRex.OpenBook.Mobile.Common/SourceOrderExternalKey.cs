// <copyright file="SourceOrderExternalKey.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound external account identifier.
    /// </summary>
    public class SourceOrderExternalKey
    {
        /// <summary>
        /// Gets or sets the mnemonic account identifier.
        /// </summary>
        public string ExternalId { get; set; }
    }
}
