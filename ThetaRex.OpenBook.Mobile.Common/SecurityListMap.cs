// <copyright file="SecurityListMap.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// A proposed order.
    /// </summary>
    public class SecurityListMap
    {
        /// <summary>
        /// Gets or sets the security list id.
        /// </summary>
        public int SecurityListId { get; set; }

        /// <summary>
        /// Gets or sets the security id.
        /// </summary>
        public int SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the unique security identifier.
        /// </summary>
        public int SecurityListMapId { get; set; }

        /// <summary>
        /// Gets or sets the time-in-force code.
        /// </summary>
        public long RowVersion { get; set; }
    }
}