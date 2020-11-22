// <copyright file="Alert.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Newtonsoft.Json;
    using ThetaRex.OpenBook.Common;

    /// <summary>
    /// A proposed order.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// Gets or sets the unique account identifier.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique alert identifier.
        /// </summary>
        public int AlertId { get; set; }

        /// <summary>
        /// Gets or sets the unique account rule parameter identifier.
        /// </summary>
        public int AccountRuleParameterId { get; set; }

        /// <summary>
        /// Gets or sets the alert's message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the alert severity.
        /// </summary>
        public Severity Severity { get; set; }
    }
}