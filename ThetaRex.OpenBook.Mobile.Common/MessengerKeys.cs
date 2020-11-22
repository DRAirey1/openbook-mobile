// <copyright file="MessengerKeys.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    /// <summary>
    /// The keys used to publish and subscribe to messages.
    /// </summary>
    public static class MessengerKeys
    {
        /// <summary>
        /// Gets the reset scenario message key.
        /// </summary>
        public static string ResetScenario { get; } = nameof(MessengerKeys.ResetScenario);
    }
}