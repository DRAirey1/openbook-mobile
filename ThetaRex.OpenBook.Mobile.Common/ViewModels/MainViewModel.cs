// <copyright file="MainViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Localization;
    using ThetaRex.Common;
    using ThetaRex.Common.ViewModels;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        /// <summary>
        /// The client used for communication to the web service.
        /// </summary>
        private readonly HttpClient<OpenBookHost> httpClient;

        /// <summary>
        /// Provides navigation for the view model.
        /// </summary>
        private readonly Navigator navigator;

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer<MainViewModel> stringLocalizer;

        /// <summary>
        /// A value indicating whether the master page is presented or not.
        /// </summary>
        private bool isPresented;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the service.</param>
        /// <param name="navigator">Provides navigation for the view model.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public MainViewModel(HttpClient<OpenBookHost> httpClient, Navigator navigator, IStringLocalizer<MainViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.httpClient = httpClient;
            this.navigator = navigator;
            this.stringLocalizer = stringLocalizer;

            // Localize the object.
            this.Exit = this.stringLocalizer["ExitLabel"];
            this.Retry = this.stringLocalizer["RetryLabel"];
            this.ServiceNotRunning = this.stringLocalizer["ServiceNotRunningMessage"];

            // Provide the menu items and their associated commands.
            this.MenuItems.Add(
                new MenuItemViewModel
                {
                    Command = new Command(pt => this.SetRoot(typeof(RootViewModel))),
                    Image = "scenarios.png",
                    Label = this.stringLocalizer["ScenariosLabel"],
                });
            this.MenuItems.Add(
                new MenuItemViewModel
                {
                    Command = new Command(pt => this.SetRoot(typeof(AboutViewModel))),
                    Image = "about.png",
                    Label = this.stringLocalizer["AboutLabel"],
                });
            this.MenuItems.Add(
                new MenuItemViewModel
                {
                    Command = new Command(this.SignOut),
                    Image = "signOut.png",
                    Label = this.stringLocalizer["SignOutLabel"],
                });
        }

        /// <summary>
        /// Gets the command that is executed when the item is pressed.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public Command Command { get; }

        /// <summary>
        /// Gets the command parameter that is passed when the command is executed.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public object CommandParameter { get; }

        /// <summary>
        /// Gets the text for the Exit button.
        /// </summary>
        public string Exit { get; }

        /// <summary>
        /// Gets the image on the menu item.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public string Image { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether the scenario is active or not.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the master page is presented or not.
        /// </summary>
        public bool IsPresented
        {
            get { return this.isPresented; }
            set { this.SetProperty(ref this.isPresented, value, nameof(this.IsPresented)); }
        }

        /// <summary>
        /// Gets the label on the item.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public string Label { get; }

        /// <summary>
        /// Gets the scenarios in the list view.
        /// </summary>
        public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new ObservableCollection<MenuItemViewModel>();

        /// <summary>
        /// Gets the text for the Retry button.
        /// </summary>
        public string Retry { get; }

        /// <summary>
        /// Gets the text for message that the service isn't running.
        /// </summary>
        public string ServiceNotRunning { get; }

        /// <summary>
        /// Sets the root of the master page.
        /// </summary>
        /// <param name="type">The page type that is to become the new root.</param>
        private async void SetRoot(Type type)
        {
            // Set the new root for the application.
            await this.navigator.SetRootAsync(type).ConfigureAwait(true);

            // Close the master page.
            this.IsPresented = false;
        }

        /// <summary>
        /// Signs the user out.
        /// </summary>
        private async void SignOut()
        {
            // This will clear the credentials and prompt a login.
            await this.httpClient.SignInAsync().ConfigureAwait(true);

            // Return to the main scenario page after signing out.
            await this.navigator.SetRootAsync(typeof(RootViewModel)).ConfigureAwait(true);

            // Close the master page.
            this.IsPresented = false;
        }
    }
}