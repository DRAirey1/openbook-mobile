// <copyright file="TradingViewModel.cs" company="Theta Rex, Inc.">
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
    using ThetaRex.OpenBook.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios involving a single issue.
    /// </summary>
    public class TradingViewModel : ScenarioViewModel
    {
        /// <summary>
        /// The DOW 30 Industrial index.
        /// </summary>
        private static readonly List<string> DowIndex = ResourceHelper.ReadEmbeddedFile<List<string>>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.DOW Index.json");

        /// <summary>
        /// The S and P 500 Index.
        /// </summary>
        private static readonly List<string> SpxIndex = ResourceHelper.ReadEmbeddedFile<List<string>>(Assembly.GetExecutingAssembly(), "ThetaRex.OpenBook.Mobile.Common.Data.SP Index.json");

        /// <summary>
        /// Translates external symbols into internal primary key values.
        /// </summary>
        private readonly Domain domain;

        /// <summary>
        /// The pre-calcluated basket of working orders for the Standard and Poors Index basket.
        /// </summary>
        private readonly List<WorkingOrder> dowBasket = new List<WorkingOrder>();

        /// <summary>
        /// Repository of data.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The pre-calcluated basket of working orders for the Standard and Poors Index basket.
        /// </summary>
        private readonly List<WorkingOrder> spxBasket = new List<WorkingOrder>();

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradingViewModel"/> class.
        /// </summary>
        /// <param name="domain">Data domain for the mobile application.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public TradingViewModel(Domain domain, IRepository repository, IStringLocalizer<TradingViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.domain = domain;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;

            // Localize the object.
            this.Title = this.stringLocalizer["Title"];

            // Use the data domain to pre-construct the trades used in this scenario.
            this.InitializeData();

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel scenarioSelectionViewModel) => await this.ResetScenariosAsync().ConfigureAwait(true));

            // Reset this scenario.
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

            // Import trades from Portfolio Optimizer.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearDowOrdersAsync,
                    ActiveLabel = this.stringLocalizer["ImportDowActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportDowOrders,
                    Description = this.stringLocalizer["ImportDowDescription"],
                    InactiveHandler = this.ImportDowOrdersAsync,
                    InactiveLabel = this.stringLocalizer["ImportDowInactiveLabel"],
                    Scenario = Scenario.ImportDowOrders,
                });

            // Create trades for S&P 500 Index basket.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveLabel = this.stringLocalizer["ImportDowActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportSpxOrders,
                    Description = this.stringLocalizer["ImportDowDescription"],
                    InactiveLabel = this.stringLocalizer["ImportDowInactiveLabel"],
                    ActiveHandler = this.ClearSpxOrdersAsync,
                    InactiveHandler = this.ImportSpxOrdersAsync,
                    Scenario = Scenario.ImportSpxOrders,
                });

            // Trade orders.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.SendOrders,
                    Description = this.stringLocalizer["SendOrdersDescription"],
                    IsEnabled = false,
                    InactiveHandler = this.SendOrdersAsync,
                    InactiveLabel = this.stringLocalizer["SendOrdersInactiveLabel"],
                    Scenario = Scenario.SendOrders,
                });
        }

        /// <summary>
        /// Clear the optimizer orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearDowOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.  Note that the allocations must be disabled immediately otherwise we leave open
            // the possibility of deleting the source orders while allowing for the import orders to be imported.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tell the service to delete the source orders (must be done first) and the working orders.
            var workingOrders = (IEnumerable<WorkingOrder>)scenarioItemViewModel.Data;
            if (await this.repository.BulkDeleteWorkingOrdersAsync(workingOrders).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportDowOrders].IsEnabled = true;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = true;
            this.Items[Scenario.SendOrders].IsEnabled = false;
        }

        /// <summary>
        /// Clear the imported orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearSpxOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.  Note that the allocations must be disabled immediately otherwise we leave open
            // the possibility of deleting the source orders while allowing for the import orders to be imported.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tell the service to delete the working orders.
            var workingOrders = (IEnumerable<WorkingOrder>)scenarioItemViewModel.Data;
            if (await this.repository.BulkDeleteWorkingOrdersAsync(workingOrders).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportDowOrders].IsEnabled = true;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = true;
            this.Items[Scenario.SendOrders].IsEnabled = false;
        }

        /// <summary>
        /// Import the Dow 30 ordewrs.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportDowOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // The embedded file contains the working orders for this scenario.
            var workingOrders = await this.repository.BulkAddWorkingOrdersAsync(this.dowBasket).ConfigureAwait(true);
            if (workingOrders != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = workingOrders;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportDowOrders].IsEnabled = true;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = true;
        }

        /// <summary>
        /// Import the SPX Working Orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportSpxOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // If we successfully created the working and their source orders, then change the state of the view model.
            var workingOrders = await this.repository.BulkAddWorkingOrdersAsync(this.spxBasket).ConfigureAwait(true);
            if (workingOrders != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = workingOrders;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = true;
            this.Items[Scenario.SendOrders].IsEnabled = true;
        }

        /// <summary>
        /// Initialize the data used by this view model to create an index basket.
        /// </summary>
        private void InitializeData()
        {
            // Don't let this long running task prevent the constructor from completing.
            Task task = Task.Run(() =>
            {
                // We're going to build baskets for CHINA on the GLOBAL desk.
                Blotter globalBlotter = this.domain.FindBlotter("GLOBALBLOTTER");
                Account china = this.domain.FindAccount("CHINA");
                ManagedAccount managedAccount = this.domain.FindManagedAccount("CHINA");

                // Create basket representing the DOW 30.
                foreach (string figi in TradingViewModel.DowIndex)
                {
                    Security security = this.domain.FindSecurity(figi);
                    Price price = this.domain.FindPrice(figi);
                    this.dowBasket.Add(new WorkingOrder
                    {
                        BlotterId = globalBlotter.BlotterId,
                        OrderTypeCode = OrderTypeCode.Market,
                        SecurityId = security.SecurityId,
                        SideCode = SideCode.Buy,
                        TimeInForce = TimeInForceCode.Day,
                        SourceOrders = new SourceOrder[]
                        {
                        new SourceOrder
                        {
                            AccountId = china.AccountId,
                            OrderTypeCode = OrderTypeCode.Market,
                            Quantity = Convert.ToDecimal(Math.Floor((Convert.ToDouble(managedAccount.NetAssetValue) * 0.01d) / (100.0d * Convert.ToDouble(price.ClosePrice))) * 100.0d),
                            SecurityId = security.SecurityId,
                            SideCode = SideCode.Buy,
                            TimeInForce = TimeInForceCode.Day,
                        },
                        },
                    });
                }

                // Create basket representing the S&P 500.
                Account redStone = this.domain.FindAccount("REDSTONE");
                foreach (string figi in TradingViewModel.SpxIndex)
                {
                    Security security = this.domain.FindSecurity(figi);
                    Price price = this.domain.FindPrice(figi);
                    this.spxBasket.Add(new WorkingOrder
                    {
                        BlotterId = globalBlotter.BlotterId,
                        OrderTypeCode = OrderTypeCode.Market,
                        SecurityId = security.SecurityId,
                        SideCode = SideCode.Buy,
                        TimeInForce = TimeInForceCode.Day,
                        SourceOrders = new SourceOrder[]
                        {
                        new SourceOrder
                        {
                            AccountId = redStone.AccountId,
                            OrderTypeCode = OrderTypeCode.Market,
                            Quantity = Convert.ToDecimal(Math.Floor((Convert.ToDouble(managedAccount.NetAssetValue) * 0.01d) / (100.0d * Convert.ToDouble(price.ClosePrice))) * 100.0d),
                            SecurityId = security.SecurityId,
                            SideCode = SideCode.Buy,
                            TimeInForce = TimeInForceCode.Day,
                        },
                        },
                    });
                }
            });
        }

        /// <summary>
        /// Reset the trading scenarios.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ResetScenariosAsync(ScenarioItemViewModel scenarioItemViewModel = null)
        {
            // Disable all commands while resetting.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Get a fresh copy of all the trades on the desk.
            Task<IEnumerable<Execution>> executionsTask = this.repository.GetExecutionsAsync();
            Task<IEnumerable<DestinationOrder>> destinationOrdersTask = this.repository.GetDestinationOrdersAsync();
            Task<IEnumerable<SourceOrder>> sourceOrdersTask = this.repository.GetSourceOrdersAsync();
            Task<IEnumerable<WorkingOrder>> workingOrdersTask = this.repository.GetWorkingOrdersAsync();

            // The Destination orders depend on executions.  They must be deleted together.
            Task<bool> task1 = Task.Run(async () =>
            {
                bool isExecutionDeleted = await this.repository.DeleteExecutionsAsync(await executionsTask.ConfigureAwait(false)).ConfigureAwait(false);
                bool isDestinationOrderDeleted = await this.repository.DeleteDestinationOrdersAsync(await destinationOrdersTask.ConfigureAwait(false)).ConfigureAwait(true);
                return isExecutionDeleted && isDestinationOrderDeleted;
            });

            // The source orders don't depend on anything and can be deleted immediately.
            Task<bool> task2 = this.repository.DeleteSourceOrdersAsync(await sourceOrdersTask.ConfigureAwait(true));

            // Wait for all the source orders, destination orders and executions to be deleted.
            bool isExecutionDestinationOrderDeleted = await task1.ConfigureAwait(false);
            bool isSourceOrderDeleted = await task2.ConfigureAwait(false);

            // The Destination and Source orders must be deleted before we can delete the working orders.
            bool isWorkingOrderDeleted = await this.repository.DeleteWorkingOrdersAsync(await workingOrdersTask.ConfigureAwait(false)).ConfigureAwait(true);

            // Reset the state of the Optimized Order scenario.
            ScenarioItemViewModel dowOrderViewModel = this.Items[Scenario.ImportDowOrders];
            if (dowOrderViewModel.IsActive)
            {
                dowOrderViewModel.Data = null;
                dowOrderViewModel.IsActive = false;
            }

            // Reset the state of the S&P Index scenario.
            ScenarioItemViewModel spxOrderViewModel = this.Items[Scenario.ImportSpxOrders];
            if (spxOrderViewModel.IsActive)
            {
                spxOrderViewModel.Data = null;
                spxOrderViewModel.IsActive = false;
            }

            // If everything was deleted properly, then reset the status of the buttons.
            if (isExecutionDestinationOrderDeleted && isSourceOrderDeleted && isWorkingOrderDeleted)
            {
                this.Items[Scenario.ImportDowOrders].IsEnabled = true;
                this.Items[Scenario.ImportSpxOrders].IsEnabled = true;
                this.Items[Scenario.SendOrders].IsEnabled = false;
            }

            // re-enable the commands after the reset.
            this.Items[Scenario.Reset].IsEnabled = true;
        }

        /// <summary>
        /// Send the orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task SendOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportDowOrders].IsEnabled = false;
            this.Items[Scenario.ImportSpxOrders].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Send the orders to their destinations.
            await this.repository.SendOrdersAsync().ConfigureAwait(true);

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
        }
    }
}