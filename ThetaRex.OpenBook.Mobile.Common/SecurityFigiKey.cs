// <copyright file="SecurityFigiKey.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound FIGI security identifier.
    /// </summary>
    public class SecurityFigiKey
    {
        /// <summary>
        /// Gets or sets the FIGI security identifier.
        /// </summary>
        public string Figi { get; set; }
    }
}