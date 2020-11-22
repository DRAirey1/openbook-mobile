// <copyright file="BlotterExternalKey.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound external account identifier.
    /// </summary>
    public class BlotterExternalKey
    {
        /// <summary>
        /// Gets or sets the mnemonic account identifier.
        /// </summary>
        public string Mnemonic { get; set; }
    }
}
