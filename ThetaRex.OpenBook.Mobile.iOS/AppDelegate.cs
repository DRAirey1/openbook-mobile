// <copyright file="AppDelegate.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.IOS
{
    using Foundation;
    using ThetaRex.OpenBook.Mobile;
    using ThetaRex.OpenBook.Mobile.Common;
    using UIKit;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.iOS;

    /// <summary>
    /// Used to manage the life cycle of the application.
    /// </summary>
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        /// <inheritdoc/>
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            // It's very important when debugging a remote problem to find out what version the user is running.  The About screen will display the
            // version information so a technician can start to isolate the problems.
            PackageInfo packageInfo = new PackageInfo();
            using (var nameNSString = new NSString("CFBundleName"))
            using (var versionNumberNSString = new NSString("CFBundleShortVersionString"))
            using (var buildNumberNSString = new NSString("CFBundleVersion"))
            {
                var packageVersion = NSBundle.MainBundle.InfoDictionary.ValueForKey(versionNumberNSString).ToString();
                var buildNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(buildNumberNSString).ToString();
                packageInfo.Version = $"{packageVersion}.{buildNumber}";
                packageInfo.Name = NSBundle.MainBundle.InfoDictionary.ValueForKey(nameNSString).ToString();
            }

            Forms.Init();
            this.LoadApplication(new App(packageInfo));
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}