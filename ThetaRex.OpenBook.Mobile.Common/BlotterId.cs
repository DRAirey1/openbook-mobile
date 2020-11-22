// <copyright file="BlotterId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound account identifier.
    /// </summary>
    public class BlotterId
    {
        /// <summary>
        /// Gets or sets the external identifier for an account.
        /// </summary>
        public BlotterExternalKey BlotterExternalKey { get; set; }
    }
}
