// <copyright file="SourceOrderId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The source order identifier.
    /// </summary>
    public class SourceOrderId
    {
        /// <summary>
        /// Gets or sets the external identifier for an account.
        /// </summary>
        public SourceOrderExternalKey SourceOrderExternalKey { get; set; }
    }
}
