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
        /// The FIGIs for the stocks in the restricted list for tabbacco.
        /// </summary>
        private static readonly List<string> SinStocks = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Sin Stocks.json");

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
        /// A random number generator.
        /// </summary>
        private readonly Random random = new Random();

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
                    InactiveHandler = this.ResetScenariosAsync,
                    InactiveLabel = this.stringLocalizer["ResetScenariosInactiveLabel"],
                    Scenario = Scenario.Reset,
                });

            // Import trades from Portfolio Optimizer.
            this.Items.Add(
                new ScenarioItemViewModel
                {
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
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportBulkAccount,
                    Description = this.stringLocalizer["ImportBulkDescription"],
                    InactiveHandler = this.ImportBulkAccountAsync,
                    InactiveLabel = this.stringLocalizer["ImportBulkInactiveLabel"],
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

            // The embedded file contains the source orders for this scenario.
            await this.repository.AddSourceOrdersAsync(this.singleAccountBasket).ConfigureAwait(true);

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
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

            // Create the source orders for the bulk account scenario.
            await this.repository.AddSourceOrdersAsync(this.bulkAccountBasket).ConfigureAwait(true);

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
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
                            SideCode = this.random.Next(0, 2) == 0 ? SideCode.Sell : SideCode.Buy,
                            TimeInForce = TimeInForceCode.Day,
                        });
                }

                // We don't want to clutter the blotter with a bunch of orders that look like they can be crossed.  So when we create a random order
                // for a security, all the other random orders for that security will have the same side.  This dictionary insures that each order
                // for a name has the same side as every other order for that name.
                Dictionary<string, SideCode> sideDictionary = new Dictionary<string, SideCode>();
                TradingViewModel.BulkAccount.ForEach(f => sideDictionary.Add(f, this.random.Next(0, 2) == 0 ? SideCode.Buy : SideCode.Sell));
                TradingViewModel.SinStocks.ForEach(f => sideDictionary.Add(f, this.random.Next(0, 2) == 0 ? SideCode.Buy : SideCode.Sell));

                // For the bulk account scenario, create a basket of the top 100 stocks by market value for the first 30 accounts.
                var accounts = (from a in this.domain.Accounts
                                where a.AccountTypeCode == AccountTypeCode.ManagedAccount && !TradingViewModel.RestrictedAccounts.Contains(a.Mnemonic)
                                select a)
                                .Take(100)
                                .ToList();
                for (int accountCounter = 0; accountCounter < accounts.Count; accountCounter++)
                {
                    // Create a basket of optimized orders for the next account.
                    Account account = accounts[accountCounter];

                    // Create a random basket of 50 securities that represent a optimization reblancing.
                    HashSet<Security> basket = new HashSet<Security>();
                    int basketSize = this.random.Next(20, 30);
                    while (basket.Count < basketSize)
                    {
                        int index = this.random.Next(0, TradingViewModel.BulkAccount.Count);
                        string figi = TradingViewModel.BulkAccount[index];

                        // This security is now part of the randomly constructed basket.
                        basket.Add(this.domain.FindSecurityByFigi(figi));
                    }

                    // In order to show that these baskets are going through compliance, we're going to add a stock that violates the tabacco list
                    // restriction at predictable intervals.  This should give us 1 violation for every 10 accounts.
                    if (accountCounter % 10 == 0)
                    {
                        int index = this.random.Next(0, TradingViewModel.SinStocks.Count);
                        basket.Add(this.domain.FindSecurityByFigi(TradingViewModel.SinStocks[index]));
                    }

                    // Create a source order for every issue in the basket.
                    foreach (Security security in basket)
                    {
                        Price price = this.domain.FindPriceByFigi(security.Figi);
                        this.bulkAccountBasket.Add(
                            new SourceOrder
                            {
                                AccountId = account.AccountId,
                                OrderTypeCode = OrderTypeCode.Market,
                                Quantity = this.random.Next(100, 1000),
                                SecurityId = security.SecurityId,
                                SideCode = sideDictionary[security.Figi],
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

            // Get a fresh copy of all the source orders, destination orders and executions on the desk.
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
            await task1.ConfigureAwait(false);
            await task2.ConfigureAwait(false);

            // re-enable the commands after the reset.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ImportSingleAccount].IsEnabled = true;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = true;
            this.Items[Scenario.SendOrders].IsEnabled = false;
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
            this.Items[Scenario.ImportSingleAccount].IsEnabled = false;
            this.Items[Scenario.ImportBulkAccount].IsEnabled = false;
            this.Items[Scenario.SendOrders].IsEnabled = false;
        }
    }
}