// <copyright file="SecurityId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound account identifier.
    /// </summary>
    public class SecurityId
    {
        /// <summary>
        /// Gets or sets the FIGI security identifier.
        /// </summary>
        public SecurityFigiKey SecurityFigiKey { get; set; }

        /// <summary>
        /// Gets or sets the external security identifier.
        /// </summary>
        public SecurityExternalKey SecurityExternalKey { get; set; }
    }
}