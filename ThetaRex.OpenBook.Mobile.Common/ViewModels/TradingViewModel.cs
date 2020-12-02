// <copyright file="TradingViewModel.cs" company="Theta Rex, Inc.">
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
        /// The FIGIs for the bulk account optimization.
        /// </summary>
        private static readonly List<string> BulkAccount = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Bulk Account.json");

        /// <summary>
        /// The FIGIs for the single account optimization.
        /// </summary>
        private static readonly List<string> SingleAccount = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Single Account.json");

        /// <summary>
        /// The restricted accounts used for most of the scenarios.
        /// </summary>
        private static readonly string[] RestrictedAccounts = new string[] { "REDSTONE", "PARKPLACE", "CHINA", "PIONEER", "HILLCREST" };

        /// <summary>
        /// The pre-calcluated basket of working orders for the Standard and Poors Index basket.
        /// </summary>
        private readonly List<SourceOrder> bulkAccountBasket = new List<SourceOrder>();

        /// <summary>
        /// Translates external symbols into internal primary key values.
        /// </summary>
        private readonly Domain domain;

        /// <summary>
        /// Repository of data.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The pre-calcluated basket of working orders for the Standard and Poors Index basket.
        /// </summary>
        private readonly List<SourceOrder> singleAccountBasket = new List<SourceOrder>();

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// The identity of the current user.
        /// </summary>
        private readonly User user;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradingViewModel"/> class.
        /// </summary>
        /// <param name="domain">Data domain for the mobile application.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        /// <param name="user">The identity of the current user.</param>
        public TradingViewModel(Domain domain, IRepository repository, IStringLocalizer<TradingViewModel> stringLocalizer, User user)
        {
            // Initialize the object.
            this.domain = domain;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;
            this.user = user;

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
                    ActiveLabel = this.stringLocalizer["ImportSingleActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportSingleAccount,
                    Description = this.stringLocalizer["ImportSingleDescription"],
                    InactiveHandler = this.ImportSingleAccountAsync,
                    InactiveLabel = this.stringLocalizer["ImportSingleInactiveLabel"],
                    Scenario = Scenario.ImportSingleAccount,
                });

            // Create trades for S&P 500 Index basket.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveLabel = this.stringLocalizer["ImportBulkActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportBulkAccount,
                    Description = this.stringLocalizer["ImportBulkDescription"],
                    InactiveLabel = this.stringLocalizer["ImportBulkInactiveLabel"],
                    ActiveHandler = this.ClearSpxOrdersAsync,
                    InactiveHandler = this.ImportBulkAccountAsync,
                    Scenario = Scenario.ImportBulkAccount,
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
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tell the service to delete the source orders (must be done first) and the working orders.
            var sourceOrders = (IEnumerable<SourceOrder>)scenarioItemViewModel.Data;
            if (await this.repository.DeleteSourceOrdersAsync(sourceOrders).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = true;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = true;
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
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tell the service to delete the working orders.
            var sourceOrders = (IEnumerable<SourceOrder>)scenarioItemViewModel.Data;
            if (await this.repository.DeleteSourceOrdersAsync(sourceOrders).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = true;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = true;
            this.Items[Scenario.SendOrders].IsEnabled = false;
        }

        /// <summary>
        /// Import the Dow 30 ordewrs.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportSingleAccountAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tag each of the orders with the current time and user.
            foreach (SourceOrder sourceOrder in this.singleAccountBasket)
            {
                // Timestamp the source order.
                sourceOrder.CreatedUserId = sourceOrder.ModifiedUserId = this.user.UserId;
                sourceOrder.CreatedTime = sourceOrder.ModifiedTime = DateTime.Now;
            }

            // The embedded file contains the working orders for this scenario.
            var sourceOrders = await this.repository.AddSourceOrdersAsync(this.singleAccountBasket).ConfigureAwait(true);
            if (sourceOrders != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = sourceOrders;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = true;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = true;
        }

        /// <summary>
        /// Import the bulk source orders.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportBulkAccountAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Tag each of the orders with the current time and user.
            foreach (SourceOrder sourceOrder in this.bulkAccountBasket)
            {
                // Timestamp the source order.
                sourceOrder.CreatedUserId = sourceOrder.ModifiedUserId = this.user.UserId;
                sourceOrder.CreatedTime = sourceOrder.ModifiedTime = DateTime.Now;
            }

            // If we successfully created the working and their source orders, then change the state of the view model.
            var sourceOrders = await this.repository.AddSourceOrdersAsync(this.bulkAccountBasket).ConfigureAwait(true);
            if (sourceOrders != null)
            {
                // Indicate that we've successfully swiched states.
                scenarioItemViewModel.Data = sourceOrders;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = true;
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
                // Wait for the domain to be initialized.
                this.domain.Initialized.WaitOne();

                // We're going to build baskets for CHINA.
                Account china = this.domain.FindAccount("CHINA");
                ManagedAccount managedAccount = this.domain.FindManagedAccount("CHINA");

                // For the single account scenario, create a basket representing the DOW 30.
                foreach (string figi in TradingViewModel.SingleAccount)
                {
                    // Each order will be for 1% of the NAV.
                    Security security = this.domain.FindSecurityByFigi(figi);
                    Price price = this.domain.FindPriceByFigi(figi);
                    this.singleAccountBasket.Add(
                        new SourceOrder
                        {
                            AccountId = china.AccountId,
                            OrderTypeCode = OrderTypeCode.Market,
                            Quantity = Convert.ToDecimal(Math.Floor(Convert.ToDouble(managedAccount.NetAssetValue) * 0.01d / (100.0d * Convert.ToDouble(price.ClosePrice))) * 100.0d),
                            SecurityId = security.SecurityId,
                            SideCode = SideCode.Buy,
                            TimeInForce = TimeInForceCode.Day,
                        });
                }

                // For the bulk account scenario, create a basket of the top 100 stocks by market value for the first 30 accounts.
                var accounts = (from a in this.domain.Accounts
                                where a.AccountTypeCode == AccountTypeCode.ManagedAccount && !TradingViewModel.RestrictedAccounts.Contains(a.Mnemonic)
                                select a).Take(30);
                foreach (Account account in accounts)
                {
                    managedAccount = this.domain.ManagedAccounts.Where(ma => ma.AccountId == account.AccountId).First();
                    foreach (string figi in TradingViewModel.BulkAccount)
                    {
                        // Each order will be for 0.1% of the basket NAV.
                        Security security = this.domain.FindSecurityByFigi(figi);
                        Price price = this.domain.FindPriceByFigi(figi);
                        this.bulkAccountBasket.Add(
                            new SourceOrder
                            {
                                AccountId = account.AccountId,
                                OrderTypeCode = OrderTypeCode.Market,
                                Quantity = Convert.ToDecimal(Math.Floor(Convert.ToDouble(managedAccount.NetAssetValue) * 0.001d / (100.0d * Convert.ToDouble(price.ClosePrice))) * 100.0d),
                                SecurityId = security.SecurityId,
                                SideCode = SideCode.Buy,
                                TimeInForce = TimeInForceCode.Day,
                            });
                    }
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
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Get a fresh copy of all the trades on the desk.
            Task<IEnumerable<Execution>> executionsTask = this.repository.GetExecutionsAsync();
            Task<IEnumerable<DestinationOrder>> destinationOrdersTask = this.repository.GetDestinationOrdersAsync();
            Task<IEnumerable<SourceOrder>> sourceOrdersTask = this.repository.GetSourceOrdersAsync();

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

            // Reset the state of the Optimized Order scenario.
            ScenarioItemViewModel dowOrderViewModel = this.Items[Scenario.ImportSingleAccount];
            if (dowOrderViewModel.IsActive)
            {
                dowOrderViewModel.Data = null;
                dowOrderViewModel.IsActive = false;
            }

            // Reset the state of the S&P Index scenario.
            ScenarioItemViewModel spxOrderViewModel = this.Items[Scenario.ImportBulkAccount];
            if (spxOrderViewModel.IsActive)
            {
                spxOrderViewModel.Data = null;
                spxOrderViewModel.IsActive = false;
            }

            // If everything was deleted properly, then reset the status of the buttons.
            if (isExecutionDestinationOrderDeleted && isSourceOrderDeleted)
            {
                this.Items[Scenario.ImportSingleAccount].IsEnabled = true;
                this.Items[Scenario.ImportBulkAccount].IsEnabled = true;
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
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;

            // Send the orders to their destinations.
            await this.repository.SendOrdersAsync().ConfigureAwait(true);

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
        }
    }
}