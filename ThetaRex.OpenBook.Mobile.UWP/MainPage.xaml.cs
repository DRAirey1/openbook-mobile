// <copyright file="MainPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.UWP
{
    using ThetaRex.OpenBook.Mobile.Common;
    using Windows.ApplicationModel;

    /// <summary>
    /// Starting page for UWP applications.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            // Initialize the XAML resources.
            this.InitializeComponent();

            // It's very important when debugging a remote problem to find out what version the user is running.  The About screen will display
            // the version information so a technician can start to isolate the problems.
            PackageVersion packageVersion = Package.Current.Id.Version;
            PackageInfo packageInfo = new PackageInfo
            {
                Name = Package.Current.DisplayName,
                Version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}",
            };

            // Load and execute the application.
            this.LoadApplication(new ThetaRex.OpenBook.Mobile.App(packageInfo));
        }
    }
}