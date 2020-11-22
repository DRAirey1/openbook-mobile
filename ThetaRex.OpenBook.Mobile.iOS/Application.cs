// <copyright file="Application.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace TheatRex.OpenBook.Mobile.IOS
{
    using UIKit;

    /// <summary>
    /// Application entry point.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}