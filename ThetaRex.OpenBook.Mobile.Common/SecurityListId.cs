// <copyright file="SecurityListId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The external classification external identifier.
    /// </summary>
    public class SecurityListId
    {
        /// <summary>
        /// Gets or sets security list unique external identifier.
        /// </summary>
        public SecurityListExternalKey SecurityListExternalKey { get; set; }
    }
}