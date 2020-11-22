// <copyright file="BenchmarkId.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// The external classification external identifier.
    /// </summary>
    public class BenchmarkId
    {
        /// <summary>
        /// Gets or sets external classification scheme unique identifier.
        /// </summary>
        public BenchmarkExternalKey BenchmarkExternalKey { get; set; }
    }
}