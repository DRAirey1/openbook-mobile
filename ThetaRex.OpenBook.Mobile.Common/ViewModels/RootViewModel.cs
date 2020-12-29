// <copyright file="RootViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Select a major scenario.
    /// </summary>
    public class RootViewModel : ScenarioViewModel
    {
        /// <summary>
        /// The shared data domain.
        /// </summary>
        private readonly Domain domain;

        /// <summary>
        /// The navigation interface for selecting scenarios.
        /// </summary>
        private readonly Navigator navigator;

        /// <summary>
        /// The data repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootViewModel"/> class.
        /// </summary>
        /// <param name="domain">The shared data domain.</param>
        /// <param name="navigator">The view model based navigation.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public RootViewModel(Domain domain, Navigator navigator, IRepository repository, IStringLocalizer<RootViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.domain = domain;
            this.navigator = navigator;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;

            // Localize the view model.
            this.Title = this.stringLocalizer["Title"];

            // Reset all the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(() => this.ResetAllScenarios()),
                    Description = this.stringLocalizer["ResetScenarioDescription"],
                    InactiveLabel = this.stringLocalizer["ResetScenarioInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.Reset,
                });

            // IBOR Scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(IborViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["IborScenarioDescription"],
                    InactiveLabel = this.stringLocalizer["IborScenarioInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.GoToIbor,
                });

            // Single security scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(SingleAccountViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["SingleAccountOperationsDescription"],
                    InactiveLabel = this.stringLocalizer["SingleAccountOperationsInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.GoToSingleAccount,
                });

            // Bulk security scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(BulkAccountViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["BulkAccountDescription"],
                    InactiveLabel = this.stringLocalizer["BulkAccountInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.GoToBulkAccount,
                });

            // Parameter change scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(RuleParameterViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["RuleParametersDescription"],
                    InactiveLabel = this.stringLocalizer["RuleParametersInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.GoToRuleParameters,
                });

            // Parameter change scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(TradingViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["TradingDescription"],
                    InactiveLabel = this.stringLocalizer["TradingInactiveLabel"],
                    IsEnabled = false,
                    Scenario = Scenario.GoToTrading,
                });

            // Add the items that invoke the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.navigator.PushAsync(typeof(ChangePriceViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["ChangePriceDescription"],
                    InactiveLabel = this.stringLocalizer["ChangePriceLabel"],
                });

            // When the data domain is initialized, we can enable the scenarios that depend on web service data.
            Task task = Task.Run(() =>
            {
                this.domain.Initialized.WaitOne();
                Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    this.Items[Scenario.Reset].IsEnabled = true;
                    this.Items[Scenario.GoToIbor].IsEnabled = true;
                    this.Items[Scenario.GoToSingleAccount].IsEnabled = true;
                    this.Items[Scenario.GoToBulkAccount].IsEnabled = true;
                    this.Items[Scenario.GoToRuleParameters].IsEnabled = true;
                    this.Items[Scenario.GoToTrading].IsEnabled = true;
                });
            });
        }

        /// <summary>
        /// Reset the all scenarios.
        /// </summary>
        private async void ResetAllScenarios()
        {
            // Disable all functions while the master reset is active.
            this.Items[Scenario.GoToBulkAccount].IsEnabled = false;
            this.Items[Scenario.GoToIbor].IsEnabled = false;
            this.Items[Scenario.GoToRuleParameters].IsEnabled = false;
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.GoToSingleAccount].IsEnabled = false;
            this.Items[Scenario.GoToTrading].IsEnabled = false;

            // Delete all the allocations.
            IEnumerable<Allocation> allocations = await this.repository.GetAllocationsAsync().ConfigureAwait(true);
            await this.repository.DeleteAllocationsAsync(allocations).ConfigureAwait(true);

            // Delete all the executions.
            IEnumerable<Execution> executions = await this.repository.GetExecutionsAsync().ConfigureAwait(true);
            await this.repository.DeleteExecutionsAsync(executions).ConfigureAwait(true);

            // Delete all the destination orders.
            IEnumerable<DestinationOrder> destinationOrders = await this.repository.GetDestinationOrdersAsync().ConfigureAwait(true);
            await this.repository.DeleteDestinationOrdersAsync(destinationOrders).ConfigureAwait(true);

            // Delete all the source orders.
            IEnumerable<SourceOrder> sourceOrders = await this.repository.GetSourceOrdersAsync().ConfigureAwait(true);
            await this.repository.DeleteSourceOrdersAsync(sourceOrders).ConfigureAwait(true);

            // Delete all the proposed orders.
            IEnumerable<ProposedOrder> proposedOrders = await this.repository.GetProposedOrdersAsync().ConfigureAwait(true);
            await this.repository.DeleteProposedOrdersAsync(proposedOrders).ConfigureAwait(true);

            // Delete all the tax lots except for the null tax lots in the 'China' account that are used as position placeholders.
            Account account = await this.repository.GetAccountByAccountExternalKeyAsync("CHINA").ConfigureAwait(true);
            Security cash = await this.repository.GetSecurityByExternalIdAsync("USD").ConfigureAwait(true);
            var taxLots = from tl in await this.repository.GetTaxLotsAsync(account.AccountId).ConfigureAwait(true)
                          where tl.Quantity != 0.0m && tl.SecurityId != cash.SecurityId
                          select tl;
            await this.repository.DeleteTaxLotsAsync(taxLots).ConfigureAwait(true);

            // Remove any items added to the Restricted Security List mappings for PFE.
            Security pfizer = await this.repository.GetSecurityByFigiAsync("BBG000BR2F10").ConfigureAwait(true);
            IEnumerable<SecurityListMap> securityListMaps = await this.repository.GetSecurityListMapsAsync().ConfigureAwait(true);
            if (pfizer != null && securityListMaps != null)
            {
                var pfizerMaps = securityListMaps.Where(slm => slm.SecurityId == pfizer.SecurityId);
                await this.repository.DeleteSecurityListMapsAsync(pfizerMaps).ConfigureAwait(true);
            }

            // Reset the Category Benchmark to be 14%.
            var categoryBenchmark = await this.repository.GetCategoryBenchmarkByCategoryBenchmarkExternalKeyAsync("DOWSICSIC.52").ConfigureAwait(true);
            categoryBenchmark.Weight = 0.14m;
            await this.repository.UpdateClassificationBenchmarkAsync(new CategoryBenchmark[] { categoryBenchmark }).ConfigureAwait(true);

            // Broadcast the reset message asking each of the view models to reset their scenarios.
            MessagingCenter.Send(this, MessengerKeys.ResetScenario);

            // re-enable all functions after the reset is complete.
            this.Items[Scenario.GoToBulkAccount].IsEnabled = true;
            this.Items[Scenario.GoToIbor].IsEnabled = true;
            this.Items[Scenario.GoToRuleParameters].IsEnabled = true;
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.GoToSingleAccount].IsEnabled = true;
            this.Items[Scenario.GoToTrading].IsEnabled = true;
        }
    }
}