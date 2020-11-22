// <copyright file="Scenario.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    /// <summary>
    /// The scenarios in this view model.
    /// </summary>
    public enum Scenario
    {
        /// <summary>
        /// Buy AAPL
        /// </summary>
        BuyAapl,

        /// <summary>
        /// Buy BRK.A
        /// </summary>
        BuyBrkA,

        /// <summary>
        /// Buy BRK.B
        /// </summary>
        BuyBrkB,

        /// <summary>
        /// Buy C
        /// </summary>
        BuyC,

        /// <summary>
        /// Buy deriviate
        /// </summary>
        BuyDerivative,

        /// <summary>
        /// Add Pfizer to restricted list.
        /// </summary>
        AddPfe,

        /// <summary>
        /// Buy PM
        /// </summary>
        BuyPm,

        /// <summary>
        /// Buy QQQ
        /// </summary>
        BuyQqq,

        /// <summary>
        /// Buy SAM
        /// </summary>
        BuySam,

        /// <summary>
        /// Buy TAP
        /// </summary>
        BuyTap,

        /// <summary>
        /// Deposit cash.
        /// </summary>
        DepositCash,

        /// <summary>
        /// Execute a block order.
        /// </summary>
        ExecuteBlockOrder,

        /// <summary>
        /// Navigate to the IBOR scenarios.
        /// </summary>
        GoToIbor,

        /// <summary>
        /// Naviate to the Bulk Account scenarios.
        /// </summary>
        GoToBulkAccount,

        /// <summary>
        /// Navigate to the Rule Parameter scenarios.
        /// </summary>
        GoToRuleParameters,

        /// <summary>
        /// Navigate to the Single Account scenarios.
        /// </summary>
        GoToSingleAccount,

        /// <summary>
        /// Navigate to the trading scenarios.
        /// </summary>
        GoToTrading,

        /// <summary>
        /// Import Allocations.
        /// </summary>
        ImportAllocations,

        /// <summary>
        /// Import a basket.
        /// </summary>
        ImportBasket,

        /// <summary>
        /// Import the DOW 30 orders.
        /// </summary>
        ImportDowOrders,

        /// <summary>
        /// Import Proposed Orders.
        /// </summary>
        ImportProposedOrders,

        /// <summary>
        /// Import Source Orders.
        /// </summary>
        ImportSourceOrders,

        /// <summary>
        /// Import the S &amp; P 500 orders.
        /// </summary>
        ImportSpxOrders,

        /// <summary>
        /// Import Tax Lots.
        /// </summary>
        ImportTaxLots,

        /// <summary>
        /// Reset the scenario.
        /// </summary>
        Reset,

        /// <summary>
        /// Send the orders on the desk to the broker.
        /// </summary>
        SendOrders,

        /// <summary>
        /// Set the industry concentration to 11%.
        /// </summary>
        SetConcentrationTo11,

        /// <summary>
        /// Set the industry concentration to 12%.
        /// </summary>
        SetConcentrationTo12,

        /// <summary>
        /// Set the industry concentration to 13%.
        /// </summary>
        SetConcentrationTo13,
    }
}