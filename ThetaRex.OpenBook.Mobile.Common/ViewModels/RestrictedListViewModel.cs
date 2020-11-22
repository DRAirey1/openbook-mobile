// <copyright file="RestrictedListViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Manages the securities in a restricted list.
    /// </summary>
    public class RestrictedListViewModel : ScenarioViewModel
    {
        /// <summary>
        /// Repository of data.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictedListViewModel"/> class.
        /// </summary>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public RestrictedListViewModel(IRepository repository, IStringLocalizer<RestrictedListViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;

            // Localist the object.
            this.Title = this.stringLocalizer["Title"];

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel scenarioSelectionViewModel) => await this.ResetScenariosAsync().ConfigureAwait(true));

            // Reset the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.Reset,
                    Description = this.stringLocalizer["ResetScenariosDescription"],
                    InactiveHandler = this.ResetScenariosAsync,
                    InactiveLabel = this.stringLocalizer["ResetScenariosInactiveLabel"],
                    Scenario = Scenario.Reset,
                });

            // PFE - Add/Remove Pfizer to restricted list.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeletePfizerAsync,
                    ActiveLabel = this.stringLocalizer["AddPfeActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.AddPfe,
                    Description = this.stringLocalizer["AddPfeDescription"],
                    InactiveHandler = this.AddPfizerAsync,
                    InactiveLabel = this.stringLocalizer["AddPfeInactiveLabel"],
                    Scenario = Scenario.AddPfe,
                });
        }

        /// <summary>
        /// Add Pfizer to the restricted list.
        /// </summary>
        private async Task AddPfizerAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.AddPfe].IsEnabled = false;

            // Add PFE to the restricted list.
            SecurityListMapRequest securityListMapRequest = new SecurityListMapRequest
            {
                SecurityListId = new SecurityListId { SecurityListExternalKey = new SecurityListExternalKey { Mnemonic = "TOBACCOSIN" } },
                SecurityId = new SecurityId { SecurityFigiKey = new SecurityFigiKey { Figi = "BBG000BR2F10" } },
            };
            var securityListMaps = await this.repository.AddSecurityListMapsAsync(new SecurityListMapRequest[] { securityListMapRequest }).ConfigureAwait(true);
            if (securityListMaps != null)
            {
                scenarioItemViewModel.Data = securityListMaps;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.AddPfe].IsEnabled = true;
        }

        /// <summary>
        /// Add Pfizer to the restricted list.
        /// </summary>
        private async Task DeletePfizerAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.AddPfe].IsEnabled = false;

            // Remove PFE from the restricted list.
            var securityListMaps = scenarioItemViewModel.Data as IEnumerable<SecurityListMap>;
            if (await this.repository.DeleteSecurityListMapsAsync(securityListMaps).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.AddPfe].IsEnabled = true;
        }

        /// <summary>
        /// Reset the single issue scenarios.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ResetScenariosAsync(ScenarioItemViewModel scenarioItemViewModel = null)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.AddPfe].IsEnabled = false;

            // Ask the web service to delete the allocations.
            ScenarioItemViewModel addPfeViewModel = this.Items[Scenario.AddPfe];
            if (addPfeViewModel.IsActive)
            {
                // Remove PFE from the restricted list.
                var securityListMaps = addPfeViewModel.Data as IEnumerable<SecurityListMap>;
                if (await this.repository.DeleteSecurityListMapsAsync(securityListMaps).ConfigureAwait(true))
                {
                    addPfeViewModel.Data = null;
                    addPfeViewModel.IsActive = false;
                }
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.AddPfe].IsEnabled = true;
        }
    }
}