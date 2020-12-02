// <copyright file="IRepository.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the repository of data.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Adds allocations.
        /// </summary>
        /// <param name="allocations">A collection of requests to create an allocation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Allocation>> AddAllocationsAsync(IEnumerable<Allocation> allocations, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds proposed orders.
        /// </summary>
        /// <param name="proposedOrders">A batch of requests to create a proposed order.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<ProposedOrder>> AddProposedOrdersAsync(IEnumerable<ProposedOrder> proposedOrders, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a security to a security list.
        /// </summary>
        /// <param name="securityListMaps">A collection of requests to add a security to a list.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<SecurityListMap>> AddSecurityListMapsAsync(IEnumerable<SecurityListMap> securityListMaps, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a list of <see cref="SourceOrder"/>s.
        /// </summary>
        /// <param name="sourceOrders">A batch of requests to create a working order.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<SourceOrder>> AddSourceOrdersAsync(IEnumerable<SourceOrder> sourceOrders, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds tax lots.
        /// </summary>
        /// <param name="taxLots">A collection of requests to create a tax lot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<TaxLot>> AddTaxLotsAsync(IEnumerable<TaxLot> taxLots, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes allocations.
        /// </summary>
        /// <param name="allocations">A list of allocations to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteAllocationsAsync(IEnumerable<Allocation> allocations, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of alerts.
        /// </summary>
        /// <param name="alerts">A list of alerts to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteAlertsAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of destination orders.
        /// </summary>
        /// <param name="destinationOrders">A list of destination orders to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteDestinationOrdersAsync(IEnumerable<DestinationOrder> destinationOrders, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of destination orders.
        /// </summary>
        /// <param name="executions">A list of destination orders to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteExecutionsAsync(IEnumerable<Execution> executions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of proposed orders.
        /// </summary>
        /// <param name="proposedOrders">A list of proposed orders to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteProposedOrdersAsync(IEnumerable<ProposedOrder> proposedOrders, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of security list mappings.
        /// </summary>
        /// <param name="securityListMaps">A mapping of a security to a list.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteSecurityListMapsAsync(IEnumerable<SecurityListMap> securityListMaps, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of source orders.
        /// </summary>
        /// <param name="sourceOrders">A list of source orders to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteSourceOrdersAsync(IEnumerable<SourceOrder> sourceOrders, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a list of proposed orders.
        /// </summary>
        /// <param name="taxLots">A list of proposed orders to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> DeleteTaxLotsAsync(IEnumerable<TaxLot> taxLots, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an account by the unique mnemonic.
        /// </summary>
        /// <param name="mnemonic">A unique external identifier of the account.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Account> GetAccountByAccountExternalKeyAsync(string mnemonic, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the accounts.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Account>> GetAccountsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the alerts.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the allocations.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Allocation>> GetAllocationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the blotters.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Blotter>> GetBlottersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the blotters.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Broker>> GetBrokersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the destination orders.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<DestinationOrder>> GetDestinationOrdersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the executions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Execution>> GetExecutionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an category benchmark by the unique external identifier.
        /// </summary>
        /// <param name="externalId">A unique external identifier of the category benchmark.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<CategoryBenchmark> GetCategoryBenchmarkByCategoryBenchmarkExternalKeyAsync(string externalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the managed accounts.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<ManagedAccount>> GetManagedAccountsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the security prices.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Price>> GetPricesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a price by the external code.
        /// </summary>
        /// <param name="externalId">The unique external identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Price> GetPriceByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a price by FIGI code.
        /// </summary>
        /// <param name="figi">The unique FIGI identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Price> GetPriceByFigiAsync(string figi, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the proposed orders.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<ProposedOrder>> GetProposedOrdersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Security>> GetSecuritiesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a security by the external code.
        /// </summary>
        /// <param name="externalId">The unique external identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Security> GetSecurityByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a security by FIGI code.
        /// </summary>
        /// <param name="figi">The unique FIGI identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Security> GetSecurityByFigiAsync(string figi, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the security list maps.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<SecurityList>> GetSecurityListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the security list maps.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<SecurityListMap>> GetSecurityListMapsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the source orders.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<SourceOrder>> GetSourceOrdersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the tax lots.
        /// </summary>
        /// <param name="accountId">The unique account identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<TaxLot>> GetTaxLotsAsync(int accountId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Send orders to the destionation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendOrdersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the category weight for the given benchmark.
        /// </summary>
        /// <param name="categoryBenchmarks">A collection of category benchmark weight records.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<CategoryBenchmark>> UpdateClassificationBenchmarkAsync(IEnumerable<CategoryBenchmark> categoryBenchmarks, CancellationToken cancellationToken = default);
    }
}