// <copyright file="AboutViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using Microsoft.Extensions.Localization;
    using ThetaRex.Common.ViewModels;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class AboutViewModel : ViewModel
    {
        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="localizer">The string localizer.</param>
        /// <param name="packageInfo">Information about the packaging of the application.</param>
        public AboutViewModel(IStringLocalizer<AboutViewModel> localizer, PackageInfo packageInfo)
        {
            // Validate the argument.
            if (packageInfo == null)
            {
                throw new ArgumentNullException(nameof(packageInfo));
            }

            // Initialize the object.
            this.localizer = localizer;
            this.Description = this.localizer["Description"];
            this.Name = packageInfo.Name;
            this.Version = packageInfo.Version;
            this.Title = this.localizer["Title"];
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the title of the view.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; }
    }
}