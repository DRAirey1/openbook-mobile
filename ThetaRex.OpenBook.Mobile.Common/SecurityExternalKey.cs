// <copyright file="SecurityExternalKey.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound FIGI security identifier.
    /// </summary>
    public class SecurityExternalKey
    {
        /// <summary>
        /// Gets or sets the FIGI security identifier.
        /// </summary>
        public string ExternalId { get; set; }
    }
}