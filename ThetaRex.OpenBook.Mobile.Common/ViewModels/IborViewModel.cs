// <copyright file="IborViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using Newtonsoft.Json.Linq;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios involving a single issue.
    /// </summary>
    public class IborViewModel : ScenarioViewModel
    {
        /// <summary>
        /// The data domain.
        /// </summary>
        private readonly Domain domain;

        /// <summary>
        /// Repository of data.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// The identity of the current user.
        /// </summary>
        private readonly User user;

        /// <summary>
        /// Initializes a new instance of the <see cref="IborViewModel"/> class.
        /// </summary>
        /// <param name="domain">The data domain.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        /// <param name="user">The identity of the current user.</param>
        public IborViewModel(Domain domain, IRepository repository, IStringLocalizer<IborViewModel> stringLocalizer, User user)
        {
            // Initialize the object.
            this.domain = domain;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;
            this.user = user;

            // Localist the object.
            this.Title = this.stringLocalizer["Title"];

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel rootViewModel) => await this.ResetScenariosAsync().ConfigureAwait(true));

            // Reset all the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.Reset,
                    Description = this.stringLocalizer["ResetScenariosDescription"],
                    InactiveLabel = this.stringLocalizer["ResetScenariosInactiveLabel"],
                    InactiveHandler = this.ResetScenariosAsync,
                    Scenario = Scenario.Reset,
                });

            // Import from accounting system.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearTaxLotAsync,
                    ActiveLabel = this.stringLocalizer["TaxLotsActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportTaxLots,
                    Description = this.stringLocalizer["TaxLotsDescription"],
                    InactiveLabel = this.stringLocalizer["TaxLotsInactiveLabel"],
                    InactiveHandler = this.ImportTaxLotAsync,
                    Scenario = Scenario.ImportTaxLots,
                });

            // Import from optimizer.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["ProposedActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportProposedOrders,
                    Description = this.stringLocalizer["ProposedDescription"],
                    InactiveLabel = this.stringLocalizer["ProposedInactiveLabel"],
                    InactiveHandler = this.ImportProposedOrderAsync,
                    Scenario = Scenario.ImportProposedOrders,
                });

            // Import from OMS/EMS.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearSourceOrderAsync,
                    ActiveLabel = this.stringLocalizer["OrdersActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportSourceOrders,
                    Description = this.stringLocalizer["OrdersDescription"],
                    InactiveLabel = this.stringLocalizer["OrdersInactiveLabel"],
                    InactiveHandler = this.ImportSourceOrderAsync,
                    Scenario = Scenario.ImportSourceOrders,
                });

            // Import from EMS.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearAllocationAsync,
                    ActiveLabel = this.stringLocalizer["AllocationsActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportAllocations,
                    IsEnabled = false,
                    Description = this.stringLocalizer["AllocationsDescription"],
                    InactiveLabel = this.stringLocalizer["AllocationsInactiveLabel"],
                    InactiveHandler = this.ImportAllocationAsync,
                    Scenario = Scenario.ImportAllocations,
                });
        }

        /// <summary>
        /// Delete allocations.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearAllocationAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportAllocations].IsEnabled = false;

            // Ask the web service to delete the allocations.
            if (await this.repository.DeleteAllocationsAsync(scenarioItemViewModel.Data as List<Allocation>).ConfigureAwait(true))
            {
                // Update the state of this scenario if successful.
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;

                // After all the allocations have been deleted, we can allow the source orders to be deleted.
                this.Items[Scenario.ImportSourceOrders].IsEnabled = true;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportAllocations].IsEnabled = true;
        }

        /// <summary>
        /// Delete proposed orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearProposedOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportProposedOrders].IsEnabled = false;

            // Delete the proposed orders.
            if (await this.repository.DeleteProposedOrdersAsync(scenarioItemViewModel.Data as List<ProposedOrder>).ConfigureAwait(true))
            {
                // Update the state of this scenario if successful.
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportProposedOrders].IsEnabled = true;
        }

        /// <summary>
        /// Delete allocations.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearSourceOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.  Note that the allocations must be disabled immediately otherwise we leave open
            // the possibility of deleting the source orders while allowing for the import orders to be imported.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = false;
            this.Items[Scenario.ImportAllocations].IsEnabled = false;

            // Tell the service to delete the source orders (must be done first) and the working orders.
            var sourceOrders = scenarioItemViewModel.Data as List<SourceOrder>;
            var isSourceOrderDeleted = await this.repository.DeleteSourceOrdersAsync(sourceOrders).ConfigureAwait(true);

            // Reset the view model if the calls to clear the working and source orders was successful.
            if (isSourceOrderDeleted && isSourceOrderDeleted)
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }
            else
            {
                // If we fail to delete the source orders, we can re-enable the allocations scenario.
                this.Items[Scenario.ImportAllocations].IsEnabled = true;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = true;
        }

        /// <summary>
        /// Delete tax lots.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearTaxLotAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportTaxLots].IsEnabled = false;

            // Tell the server to delete tax lots we got.
            if (await this.repository.DeleteTaxLotsAsync(scenarioItemViewModel.Data as List<TaxLot>).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportTaxLots].IsEnabled = true;
        }

        /// <summary>
        /// Import Allocations.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportAllocationAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportAllocations].IsEnabled = false;

            // The embedded file contains the proposed for this scenario.
            var allocations = new List<Allocation>();
            var jArray = ResourceHelper.ReadEmbeddedFile<JArray>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.China Allocation.json");
            foreach (JObject jObject in jArray)
            {
                // Translate the account id.
                var accountIdObject = jObject.GetValue("accountId", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKey = accountIdObject.GetValue("accountExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKeyMnemonic = (string)accountExternalKey.GetValue("mnemonic", StringComparison.OrdinalIgnoreCase);
                accountIdObject.Replace(new JValue(this.domain.FindAccount(accountExternalKeyMnemonic).AccountId));

                // Translate the broker id.
                var brokerIdObject = jObject.GetValue("brokerId", StringComparison.OrdinalIgnoreCase) as JObject;
                var brokerSymbolKey = brokerIdObject.GetValue("brokerSymbolKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var brokerbrokerSymbolKeySymbol = (string)brokerSymbolKey.GetValue("symbol", StringComparison.OrdinalIgnoreCase);
                brokerIdObject.Replace(new JValue(this.domain.FindBroker(brokerbrokerSymbolKeySymbol).BrokerId));

                // Translate the security id.
                var securityIdObject = jObject.GetValue("securityId", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKey = securityIdObject.GetValue("securityFigiKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKeyFigi = (string)securityFigiKey.GetValue("figi", StringComparison.OrdinalIgnoreCase);
                securityIdObject.Replace(new JValue(this.domain.FindSecurityByFigi(securityFigiKeyFigi).SecurityId));

                // Translate the settlement security (currency) id.
                var settlementIdObject = jObject.GetValue("settlementId", StringComparison.OrdinalIgnoreCase) as JObject;
                var settlementExternalKey = settlementIdObject.GetValue("securityExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var settlementExternalId = (string)settlementExternalKey.GetValue("externalId", StringComparison.OrdinalIgnoreCase);
                settlementIdObject.Replace(new JValue(this.domain.FindSecurityByExternalId(settlementExternalId).SecurityId));

                // Translate the source order identifier.
                var sourceOrderIdObject = jObject.GetValue("sourceOrderId", StringComparison.OrdinalIgnoreCase) as JObject;
                var sourceOrderExternalKey = sourceOrderIdObject.GetValue("sourceOrderExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var sourceOrderExternalKeyExternalId = (string)sourceOrderExternalKey.GetValue("externalId", StringComparison.OrdinalIgnoreCase);
                ScenarioItemViewModel sourceOrdersScenario = this.Items.Where(svm => svm.Scenario == Scenario.ImportSourceOrders).First();
                var sourceOrders = sourceOrdersScenario.Data as List<SourceOrder>;
                var sourceOrder = sourceOrders.Where(so => so.ExternalId == sourceOrderExternalKeyExternalId).First();
                sourceOrderIdObject.Replace(new JValue(sourceOrder.SourceOrderId));

                // Timestamp the proposed order.
                jObject.Add("CreatedUserId", this.user.UserId);
                jObject.Add("CreatedTime", DateTime.Now);
                jObject.Add("ModifiedUserId", this.user.UserId);
                jObject.Add("ModifiedTime", DateTime.Now);

                // This is now a prepared proposed order, ready to be sent to the service.
                allocations.Add(jObject.ToObject<Allocation>());
            }

            var data = await this.repository.AddAllocationsAsync(allocations).ConfigureAwait(true);
            if (data != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = data;
                scenarioItemViewModel.IsActive = true;

                // We can't allow the parent Source Orders to be cleared while we have allocations.  That would cause a breach in referential
                // integrity.
                this.Items[Scenario.ImportSourceOrders].IsEnabled = false;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportAllocations].IsEnabled = true;
        }

        /// <summary>
        /// Import Proposed Orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportProposedOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportProposedOrders].IsEnabled = false;

            // The embedded file contains the proposed for this scenario.
            var proposedOrders = new List<ProposedOrder>();
            var jArray = ResourceHelper.ReadEmbeddedFile<JArray>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.China Proposed Order.json");
            foreach (JObject jObject in jArray)
            {
                // Translate the account id.
                var accountIdObject = jObject.GetValue("accountId", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKey = accountIdObject.GetValue("accountExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKeyMnemonic = (string)accountExternalKey.GetValue("mnemonic", StringComparison.OrdinalIgnoreCase);
                accountIdObject.Replace(new JValue(this.domain.FindAccount(accountExternalKeyMnemonic).AccountId));

                // Translate the security id.
                var securityIdObject = jObject.GetValue("securityId", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKey = securityIdObject.GetValue("securityFigiKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKeyFigi = (string)securityFigiKey.GetValue("figi", StringComparison.OrdinalIgnoreCase);
                securityIdObject.Replace(new JValue(this.domain.FindSecurityByFigi(securityFigiKeyFigi).SecurityId));

                // Timestamp the proposed order.
                jObject.Add("CreatedUserId", this.user.UserId);
                jObject.Add("CreatedTime", DateTime.Now);
                jObject.Add("ModifiedUserId", this.user.UserId);
                jObject.Add("ModifiedTime", DateTime.Now);

                // This is now a prepared proposed order, ready to be sent to the service.
                proposedOrders.Add(jObject.ToObject<ProposedOrder>());
            }

            // Send the translated orders to the web service.
            var data = await this.repository.AddProposedOrdersAsync(proposedOrders).ConfigureAwait(true);
            if (data != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = data;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportProposedOrders].IsEnabled = true;
        }

        /// <summary>
        /// Import Source Orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportSourceOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = false;

            // The embedded file contains the source orders for this scenario.
            var sourceOrders = new List<SourceOrder>();
            var jArray = ResourceHelper.ReadEmbeddedFile<JArray>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.China Source Order.json");
            foreach (JObject jObject in jArray)
            {
                // Translate the account id.
                var accountIdObject = jObject.GetValue("accountId", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKey = accountIdObject.GetValue("accountExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKeyMnemonic = (string)accountExternalKey.GetValue("mnemonic", StringComparison.OrdinalIgnoreCase);
                accountIdObject.Replace(new JValue(this.domain.FindAccount(accountExternalKeyMnemonic).AccountId));

                // Translate the security id.
                var securityIdObject = jObject.GetValue("securityId", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKey = securityIdObject.GetValue("securityFigiKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKeyFigi = (string)securityFigiKey.GetValue("figi", StringComparison.OrdinalIgnoreCase);
                securityIdObject.Replace(new JValue(this.domain.FindSecurityByFigi(securityFigiKeyFigi).SecurityId));

                // Timestamp the source order.
                jObject.Add("CreatedUserId", this.user.UserId);
                jObject.Add("CreatedTime", DateTime.Now);
                jObject.Add("ModifiedUserId", this.user.UserId);
                jObject.Add("ModifiedTime", DateTime.Now);

                // This is now a prepared source order, ready to be sent to the service.
                sourceOrders.Add(jObject.ToObject<SourceOrder>());
            }

            // If we successfully created the working and their source orders, then change the state of the view model.
            var data = await this.repository.AddSourceOrdersAsync(sourceOrders).ConfigureAwait(true);
            if (data != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = data;
                scenarioItemViewModel.IsActive = true;

                // We can't load allocations until we've loaded the source orders against which those allocations can be posted.
                this.Items[Scenario.ImportAllocations].IsEnabled = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = true;
        }

        /// <summary>
        /// Import Tax Lots.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportTaxLotAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportTaxLots].IsEnabled = false;

            // The embedded file contains the tax lots for this scenario.
            var taxLots = new List<TaxLot>();
            var jArray = ResourceHelper.ReadEmbeddedFile<JArray>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.China Tax Lot.json");
            foreach (JObject jObject in jArray)
            {
                // Translate the account id.
                var accountIdObject = jObject.GetValue("accountId", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKey = accountIdObject.GetValue("accountExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKeyMnemonic = (string)accountExternalKey.GetValue("mnemonic", StringComparison.OrdinalIgnoreCase);
                accountIdObject.Replace(new JValue(this.domain.FindAccount(accountExternalKeyMnemonic).AccountId));

                // Translate the security id.
                var securityIdObject = jObject.GetValue("securityId", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKey = securityIdObject.GetValue("securityFigiKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var securityFigiKeyFigi = (string)securityFigiKey.GetValue("figi", StringComparison.OrdinalIgnoreCase);
                securityIdObject.Replace(new JValue(this.domain.FindSecurityByFigi(securityFigiKeyFigi).SecurityId));

                // This is now a prepared tax lot, ready to be sent to the service.
                taxLots.Add(jObject.ToObject<TaxLot>());
            }

            // Now that the external identifiers have been translated, send the tax lots to the web service.
            var data = await this.repository.AddTaxLotsAsync(taxLots).ConfigureAwait(true);
            if (data != null)
            {
                scenarioItemViewModel.Data = data;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportTaxLots].IsEnabled = true;
        }

        /// <summary>
        /// Reset the single issue scenarios.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ResetScenariosAsync(ScenarioItemViewModel scenarioItemViewModel = null)
        {
            // Disable all commands while resetting.
            this.Items[Scenario.ImportAllocations].IsEnabled = false;
            this.Items[Scenario.ImportProposedOrders].IsEnabled = false;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = false;
            this.Items[Scenario.ImportTaxLots].IsEnabled = false;
            this.Items[Scenario.Reset].IsEnabled = false;

            // Ask the web service to delete the allocations.
            ScenarioItemViewModel allocationViewModel = this.Items[Scenario.ImportAllocations];
            if (allocationViewModel.IsActive)
            {
                if (await this.repository.DeleteAllocationsAsync(allocationViewModel.Data as List<Allocation>).ConfigureAwait(true))
                {
                    allocationViewModel.Data = null;
                    allocationViewModel.IsActive = false;
                }
            }

            // Clear the source orders.
            ScenarioItemViewModel sourceOrderViewModel = this.Items[Scenario.ImportSourceOrders];
            if (sourceOrderViewModel.IsActive)
            {
                var sourceOrders = sourceOrderViewModel.Data as List<SourceOrder>;
                var isSourceOrderDeleted = await this.repository.DeleteSourceOrdersAsync(sourceOrders).ConfigureAwait(true);
                if (isSourceOrderDeleted)
                {
                    sourceOrderViewModel.Data = null;
                    sourceOrderViewModel.IsActive = false;
                }
            }

            // Clear the proposed orders.
            ScenarioItemViewModel proposedOrderViewModel = this.Items[Scenario.ImportProposedOrders];
            if (proposedOrderViewModel.IsActive)
            {
                if (await this.repository.DeleteProposedOrdersAsync(proposedOrderViewModel.Data as List<ProposedOrder>).ConfigureAwait(true))
                {
                    proposedOrderViewModel.Data = null;
                    proposedOrderViewModel.IsActive = false;
                }
            }

            // Clear the tax lots.
            ScenarioItemViewModel taxLotViewModel = this.Items[Scenario.ImportTaxLots];
            if (taxLotViewModel.IsActive)
            {
                if (await this.repository.DeleteTaxLotsAsync(taxLotViewModel.Data as List<TaxLot>).ConfigureAwait(true))
                {
                    taxLotViewModel.Data = null;
                    taxLotViewModel.IsActive = false;
                }
            }

            // re-enable the commands after the reset.
            this.Items[Scenario.ImportProposedOrders].IsEnabled = true;
            this.Items[Scenario.ImportSourceOrders].IsEnabled = true;
            this.Items[Scenario.ImportTaxLots].IsEnabled = true;
            this.Items[Scenario.Reset].IsEnabled = true;
        }
    }
}