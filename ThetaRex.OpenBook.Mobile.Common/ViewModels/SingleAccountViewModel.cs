// <copyright file="SingleAccountViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
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
    public class SingleAccountViewModel : ScenarioViewModel
    {
        /// <summary>
        /// This is a mapping of the scenarios to a file containing proposed orders for that scenario.
        /// </summary>
        private static readonly Dictionary<Scenario, string> ProposedOrderDictionary = new Dictionary<Scenario, string>()
        {
            { Scenario.BuyBrkA, "ThetaRex.OpenBook.Mobile.Common.Data.Buy BRK.A.json" },
            { Scenario.BuyBrkB, "ThetaRex.OpenBook.Mobile.Common.Data.Buy BRK.B.json" },
            { Scenario.BuyC, "ThetaRex.OpenBook.Mobile.Common.Data.Buy C.json" },
            { Scenario.DepositCash, "ThetaRex.OpenBook.Mobile.Common.Data.Buy Cash.json" },
            { Scenario.BuyPm, "ThetaRex.OpenBook.Mobile.Common.Data.Buy PM.json" },
            { Scenario.BuyQqq, "ThetaRex.OpenBook.Mobile.Common.Data.Buy QQQ.json" },
            { Scenario.BuySam, "ThetaRex.OpenBook.Mobile.Common.Data.Buy SAM.json" },
            { Scenario.BuyTap, "ThetaRex.OpenBook.Mobile.Common.Data.Buy TAP.json" },
            { Scenario.ImportBasket, "ThetaRex.OpenBook.Mobile.Common.Data.Buy Basket.json" },
        };

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
        /// Initializes a new instance of the <see cref="SingleAccountViewModel"/> class.
        /// </summary>
        /// <param name="domain">The data domain.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        /// <param name="user">The identity of the current user.</param>
        public SingleAccountViewModel(Domain domain, IRepository repository, IStringLocalizer<SingleAccountViewModel> stringLocalizer, User user)
        {
            // Initialize the object.
            this.domain = domain;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;
            this.user = user;

            // Localize the object.
            this.Title = this.stringLocalizer["Title"];

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel scenarioSelectionViewModel) => await this.ResetScenariosAsync().ConfigureAwait(true));

            // Configure the scenarios.  Clear all the single issue scenarios.
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

            // Add/Clear the tax lots for this scenario.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.ClearTaxLotAsync,
                    ActiveLabel = this.stringLocalizer["TaxLotsActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportTaxLots,
                    Description = this.stringLocalizer["TaxLotsDescription"],
                    InactiveHandler = this.ImportTaxLotAsync,
                    InactiveLabel = this.stringLocalizer["TaxLotsInactiveLabel"],
                    Scenario = Scenario.ImportTaxLots,
                });

            // Buy SAM - Violate alcohol restriction.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["SamActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuySam,
                    Description = this.stringLocalizer["SamDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["SamInactiveLabel"],
                    Scenario = Scenario.BuySam,
                });

            // Buy PM - Violate tabacco restriction.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["PmActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyPm,
                    Description = this.stringLocalizer["PmDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["PmInactiveLabel"],
                    Scenario = Scenario.BuyPm,
                });

            // Buy TAP - Violate alcohol restriction.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["TapActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyTap,
                    Description = this.stringLocalizer["TapDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["TapInactiveLabel"],
                    Scenario = Scenario.BuyTap,
                });

            // Add Cash - Violation of Cash Concentration.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["CashActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.DepositCash,
                    Description = this.stringLocalizer["CashDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["CashInactiveLabel"],
                    Scenario = Scenario.DepositCash,
                });

            // Citigroup - Violation of Industry Concentration.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["CActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyC,
                    Description = this.stringLocalizer["CDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["CInactiveLabel"],
                    Scenario = Scenario.BuyC,
                });

            // BRK.A - Violation of Issuer Concentration.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["BrkAActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyBrkA,
                    Description = this.stringLocalizer["BrkADescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["BrkAInactiveLabel"],
                    Scenario = Scenario.BuyBrkA,
                });

            // BRK.B - Violation of Issuer Concentration.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["BrkBActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyBrkB,
                    Description = this.stringLocalizer["BrkBDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["BrkBInactiveLabel"],
                    Scenario = Scenario.BuyBrkB,
                });

            // QQQ - Security Type Violation.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["QqqActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.BuyQqq,
                    Description = this.stringLocalizer["QqqDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["QqqInactiveLabel"],
                    Scenario = Scenario.BuyQqq,
                });

            // Buy a basket.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    ActiveHandler = this.DeleteProposedOrderAsync,
                    ActiveLabel = this.stringLocalizer["BasketActiveLabel"],
                    Command = new Command<Scenario>((s) => this.RouteCommand(s)),
                    CommandParameter = Scenario.ImportBasket,
                    Description = this.stringLocalizer["BasketDescription"],
                    InactiveHandler = this.BuyProposedOrdersAsync,
                    InactiveLabel = this.stringLocalizer["BasketInactiveLabel"],
                    Scenario = Scenario.ImportBasket,
                });
        }

        /// <summary>
        /// Buy a proposed order.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task BuyProposedOrdersAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            scenarioItemViewModel.IsEnabled = false;

            // The embedded file contains the proposed for this scenario.
            var proposedOrders = new List<ProposedOrder>();
            var jArray = ResourceHelper.ReadEmbeddedFile<JArray>(
                Assembly.GetExecutingAssembly(),
                SingleAccountViewModel.ProposedOrderDictionary[(Scenario)scenarioItemViewModel.CommandParameter]);
            foreach (JObject jObject in jArray)
            {
                // Translate the account id.
                var accountIdObject = jObject.GetValue("accountId", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKey = accountIdObject.GetValue("accountExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                var accountExternalKeyMnemonic = (string)accountExternalKey.GetValue("mnemonic", StringComparison.OrdinalIgnoreCase);
                accountIdObject.Replace(new JValue(this.domain.FindAccount(accountExternalKeyMnemonic).AccountId));

                // Translate the security id using either the FIGI code (for equities) or the external identifier (for currencies).
                var securityIdObject = jObject.GetValue("securityId", StringComparison.OrdinalIgnoreCase) as JObject;
                int securityId = default;
                var securityFigiKey = securityIdObject.GetValue("securityFigiKey", StringComparison.OrdinalIgnoreCase) as JObject;
                if (securityFigiKey != null)
                {
                    var securityFigiKeyFigi = (string)securityFigiKey.GetValue("figi", StringComparison.OrdinalIgnoreCase);
                    securityId = this.domain.FindSecurityByFigi(securityFigiKeyFigi).SecurityId;
                }

                var securityExternalKey = securityIdObject.GetValue("securityExternalKey", StringComparison.OrdinalIgnoreCase) as JObject;
                if (securityExternalKey != null)
                {
                    var securityExternalKeyExternalId = (string)securityExternalKey.GetValue("externalId", StringComparison.OrdinalIgnoreCase);
                    securityId = this.domain.FindSecurityByExternalId(securityExternalKeyExternalId).SecurityId;
                }

                securityIdObject.Replace(new JValue(securityId));

                // Timestamp the proposed order.
                jObject.Add("CreatedUserId", this.user.UserId);
                jObject.Add("CreatedTime", DateTime.Now);
                jObject.Add("ModifiedUserId", this.user.UserId);
                jObject.Add("ModifiedTime", DateTime.Now);

                // This is now a prepared tax lot, ready to be sent to the service.
                proposedOrders.Add(jObject.ToObject<ProposedOrder>());
            }

            // Add the translated proposed order(s) to the web service.
            scenarioItemViewModel.Data = await this.repository.AddProposedOrdersAsync(proposedOrders).ConfigureAwait(true);
            scenarioItemViewModel.IsActive = scenarioItemViewModel.Data != null;

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            scenarioItemViewModel.IsEnabled = true;
        }

        /// <summary>
        /// Deletes a Proposed Order created by a scenario.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        /// <returns>A task for awaiting.</returns>
        private async Task DeleteProposedOrderAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;
            scenarioItemViewModel.IsEnabled = false;

            try
            {
                // Delete the propsoed order.
                List<ProposedOrder> proposedOrders = scenarioItemViewModel.Data as List<ProposedOrder>;
                await this.repository.DeleteProposedOrdersAsync(proposedOrders).ConfigureAwait(true);
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }
            catch (ArgumentNullException)
            {
            }

            // Re-enable the scenario when the server is done.
            this.Items[Scenario.Reset].IsEnabled = true;
            scenarioItemViewModel.IsEnabled = true;
        }

        /// <summary>
        /// Delete tax lots.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ClearTaxLotAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.ImportTaxLots].IsEnabled = false;

            // Tell the server to delete tax lots we got.
            if (await this.repository.DeleteTaxLotsAsync(scenarioItemViewModel.Data as List<TaxLot>).ConfigureAwait(true))
            {
                scenarioItemViewModel.Data = null;
                scenarioItemViewModel.IsActive = false;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.ImportTaxLots].IsEnabled = true;
        }

        /// <summary>
        /// Import Tax Lots.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ImportTaxLotAsync(ScenarioItemViewModel scenarioItemViewModel)
        {
            // Disable while the command is executed on the server.
            this.Items[Scenario.Reset].IsEnabled = false;

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

            // Post the tax lots to the web service.
            var data = await this.repository.AddTaxLotsAsync(taxLots).ConfigureAwait(true);
            if (data != null)
            {
                scenarioItemViewModel.Data = data;
                scenarioItemViewModel.IsActive = true;
            }

            // Re-enable once the command has finished.
            this.Items[Scenario.Reset].IsEnabled = true;
        }

        /// <summary>
        /// Reset the single issue scenarios.
        /// </summary>
        /// <param name="scenarioItemViewModel">The view model of the selected scenario.</param>
        private async Task ResetScenariosAsync(ScenarioItemViewModel scenarioItemViewModel = null)
        {
            // This will run through all the scenarios and clear the active ones.
            foreach (ScenarioItemViewModel siblingItemViewModel in this.Items)
            {
                if (siblingItemViewModel.IsActive)
                {
                    await siblingItemViewModel.ActiveHandler(siblingItemViewModel).ConfigureAwait(true);
                }
            }
        }
    }
}