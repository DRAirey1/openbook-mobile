// <copyright file="IborViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios involving a single issue.
    /// </summary>
    public class IborViewModel : ScenarioViewModel
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
        /// Initializes a new instance of the <see cref="IborViewModel"/> class.
        /// </summary>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public IborViewModel(IRepository repository, IStringLocalizer<IborViewModel> stringLocalizer)
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
            var tuple = (ValueTuple<IEnumerable<WorkingOrder>, IEnumerable<SourceOrder>>)scenarioItemViewModel.Data;
            var isSourceOrderDeleted = await this.repository.DeleteSourceOrdersAsync(tuple.Item2).ConfigureAwait(true);
            var isWorkingOrderDeleted = await this.repository.DeleteWorkingOrdersAsync(tuple.Item1).ConfigureAwait(true);

            // Reset the view model if the calls to clear the working and source orders was successful.
            if (isSourceOrderDeleted && isWorkingOrderDeleted)
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

            // The embedded file contains the allocations for this scenario.
            List<AllocationRequest> allocationRequests = ResourceHelper.ReadEmbeddedFile<List<AllocationRequest>>(
                Assembly.GetExecutingAssembly(),
                "ThetaRex.OpenBook.Mobile.Common.Data.China Allocation.json");
            var data = await this.repository.AddAllocationsAsync(allocationRequests).ConfigureAwait(true);
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

            // The embedded file contains the proposed orders for this scenario.
            List<ProposedOrderRequest> proposedOrderRequests = ResourceHelper.ReadEmbeddedFile<List<ProposedOrderRequest>>(
                Assembly.GetExecutingAssembly(),
                "ThetaRex.OpenBook.Mobile.Common.Data.China Proposed Order.json");
            var data = await this.repository.AddProposedOrdersAsync(proposedOrderRequests).ConfigureAwait(true);
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

            // The embedded file contains the working orders for this scenario.
            List<WorkingOrderRequest> workingOrderRequests = ResourceHelper.ReadEmbeddedFile<List<WorkingOrderRequest>>(
                Assembly.GetExecutingAssembly(),
                "ThetaRex.OpenBook.Mobile.Common.Data.China Working Order.json");
            var workingOrders = await this.repository.AddWorkingOrdersAsync(workingOrderRequests).ConfigureAwait(true);

            // The embedded file contains the source orders for this scenario.
            List<SourceOrderRequest> sourceOrderRequests = ResourceHelper.ReadEmbeddedFile<List<SourceOrderRequest>>(
                Assembly.GetExecutingAssembly(),
                "ThetaRex.OpenBook.Mobile.Common.Data.China Source Order.json");
            var sourceOrders = await this.repository.AddSourceOrdersAsync(sourceOrderRequests).ConfigureAwait(true);

            // If we successfully created the working and their source orders, then change the state of the view model.
            if (workingOrders != null && sourceOrders != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = (workingOrders, sourceOrders);
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
            List<TaxLotRequest> taxLotRequests = ResourceHelper.ReadEmbeddedFile<List<TaxLotRequest>>(
                Assembly.GetExecutingAssembly(),
                "ThetaRex.OpenBook.Mobile.Common.Data.China Tax Lot.json");
            var data = await this.repository.AddTaxLotsAsync(taxLotRequests).ConfigureAwait(true);
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
                var tuple = (ValueTuple<IEnumerable<WorkingOrder>, IEnumerable<SourceOrder>>)sourceOrderViewModel.Data;
                var isSourceOrderDeleted = await this.repository.DeleteSourceOrdersAsync(tuple.Item2).ConfigureAwait(true);
                var isWorkingOrderDeleted = await this.repository.DeleteWorkingOrdersAsync(tuple.Item1).ConfigureAwait(true);
                if (isSourceOrderDeleted && isWorkingOrderDeleted)
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