// <copyright file="CategoryId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The compound FIGI security identifier.
    /// </summary>
    public class CategoryId
    {
        /// <summary>
        /// Gets or sets the external unique identifier.
        /// </summary>
        public CategoryExternalKey CategoryExternalKey { get; set; }
    }
}