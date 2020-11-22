// <copyright file="SecurityListExternalKey.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The external security list identifier.
    /// </summary>
    public class SecurityListExternalKey
    {
        /// <summary>
        /// Gets or sets the FIGI security identifier.
        /// </summary>
        public string Mnemonic { get; set; }
    }
}