// <copyright file="User.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    /// <summary>
    /// Defines a user's identity inside the application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identity of the current user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string Name { get; set; }
   }
}