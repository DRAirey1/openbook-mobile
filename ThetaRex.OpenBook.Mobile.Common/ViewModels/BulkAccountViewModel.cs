// <copyright file="BulkAccountViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using ThetaRex.OpenBook.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Bulk operation scenarios.
    /// </summary>
    public class BulkAccountViewModel : ScenarioViewModel
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
        /// Translates external symbols into internal primary key values.
        /// </summary>
        private readonly Domain domain;

        /// <summary>
        /// The identity of the current user.
        /// </summary>
        private readonly User user;

        /// <summary>
        /// A pre-calculated working order.
        /// </summary>
        private List<SourceOrder> sourceOrders;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkAccountViewModel"/> class.
        /// </summary>
        /// <param name="domain">Data domain for the mobile application.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        /// <param name="user">The identity of the current user.</param>
        public BulkAccountViewModel(Domain domain, IRepository repository, IStringLocalizer<BulkAccountViewModel> stringLocalizer, User user)
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

            // Configure the scenarios.  Clear all the bulk issue scenarios.
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

            // Buy a block order for many accounts.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearBlockOrderAsync,
                    ActiveLabel = this.stringLocalizer["BlockOrderActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ExecuteBlockOrder,
                    Description = this.stringLocalizer["BlockOrderDescription"],
                    InactiveHandler = this.BuyBlockOrderAsync,
                    InactiveLabel = this.stringLocalizer["BlockOrderInactiveLabel"],
                    Scenario = Scenario.ExecuteBlockOrder,
                });
        }

        /// <summary>
        /// Buy a single security for multiple accounts.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task BuyBlockOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = false;

            // Tag each of the orders with the current time and user.
            foreach (SourceOrder sourceOrder in this.sourceOrders)
            {
                // Timestamp the source order.
                sourceOrder.CreatedUserId = sourceOrder.ModifiedUserId = this.user.UserId;
                sourceOrder.CreatedTime = sourceOrder.ModifiedTime = DateTime.Now;
            }

            // Post the entire batch of working orders to the repository.
            var sourceOrders = await this.repository.AddSourceOrdersAsync(this.sourceOrders).ConfigureAwait(true);

            // Update the status of the view model.
            scenarioItemViewModel.Data = sourceOrders;
            scenarioItemViewModel.IsActive = true;

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = true;
        }

        /// <summary>
        /// Clear the block order.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearBlockOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = false;

            // Extract the lists of source orders and the working order from the generic data.
            IEnumerable<SourceOrder> workingOrders = scenarioItemViewModel.Data as IEnumerable<SourceOrder>;

            // Clear the working order.
            if (!await this.repository.DeleteSourceOrdersAsync(workingOrders).ConfigureAwait(true))
            {
                return;
            }

            // Update the status of the view model.
            scenarioItemViewModel.Data = null;
            scenarioItemViewModel.IsActive = false;

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = true;
        }

        /// <summary>
        /// Initialize the data used by this view model to create a block order.
        /// </summary>
        private void InitializeData()
        {
            // Don't let this long running task prevent the constructor from completing.
            Task task = Task.Run(() =>
            {
                // Create a large block order for American Express for all accounts.
                Security security = this.domain.FindSecurityByFigi("BBG000BCR153");
                this.sourceOrders = (from a in this.domain.Accounts
                                    join ma in this.domain.ManagedAccounts on a.AccountId equals ma.AccountId
                                    select new SourceOrder
                                    {
                                        AccountId = a.AccountId,
                                        OrderTypeCode = OrderTypeCode.Market,
                                        Quantity = this.CalculateQuantity(a, security, 0.048),
                                        SecurityId = security.SecurityId,
                                        SideCode = SideCode.Buy,
                                        TimeInForce = TimeInForceCode.Day,
                                    }).ToList();
            });
        }

        /// <summary>
        /// Calculates the quantity for a block order such that we're close to given fraction of the NAV.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="security">The security.</param>
        /// <param name="percent">The fraction of the given account's NAV.</param>
        /// <returns>A quantity that's close to the given fraction of the account's net asset value.</returns>
        private decimal CalculateQuantity(Account account, Security security, double percent)
        {
            ManagedAccount managedAccount = this.domain.FindManagedAccount(account.Mnemonic);
            Price price = this.domain.FindPriceByFigi(security.Figi);
            return Convert.ToDecimal(Math.Round(Convert.ToDouble(managedAccount.NetAssetValue) * percent / (Convert.ToDouble(price.LastPrice) * 100.0d)) * 100.0d);
        }

        /// <summary>
        /// Reset the single issue scenarios.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ResetScenariosAsync(ScenarioItemViewModel scenarioItemViewModel = null)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = false;

            // Clear the Block Order scenario.
            ScenarioItemViewModel blockOrderViewModel = this.Items[Scenario.ExecuteBlockOrder];
            if (blockOrderViewModel.IsActive)
            {
                // Extract the lists of source orders and the working order from the generic data.
                IEnumerable<SourceOrder> workingOrders = blockOrderViewModel.Data as IEnumerable<SourceOrder>;

                // Clear the working order.
                if (workingOrders != null && !await this.repository.DeleteSourceOrdersAsync(workingOrders).ConfigureAwait(true))
                {
                    return;
                }

                // Update the status of the view model.
                blockOrderViewModel.Data = null;
                blockOrderViewModel.IsActive = false;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.ExecuteBlockOrder].IsEnabled = true;
            this.Items[Scenario.Reset].IsEnabled = true;
        }
    }
}