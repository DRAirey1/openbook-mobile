// <copyright file="SecurityListMapRequest.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A request to create a proposed order.
    /// </summary>
    public class SecurityListMapRequest
    {
        /// <summary>
        /// Gets or sets security list unique identifier.
        /// </summary>
        public SecurityListId SecurityListId { get; set; }

        /// <summary>
        /// Gets or sets security unique identifier.
        /// </summary>
        public SecurityId SecurityId { get; set; }
    }
}