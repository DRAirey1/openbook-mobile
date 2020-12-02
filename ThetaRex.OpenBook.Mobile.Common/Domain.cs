// <copyright file="Domain.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using ThetaRex.Common;

    /// <summary>
    /// Used to translate external symbols into unique internal identifiers.
    /// </summary>
    public class Domain : IDisposable
    {
        /// <summary>
        /// The time to wait before retrying the initialization.
        /// </summary>
        private const int RetryTime = 10000;

        /// <summary>
        /// The tickers for the currencies in this data domain.
        /// </summary>
        private static readonly List<string> CurrencyUniverse = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Currency Universe.json");

        /// <summary>
        /// The FIGIs for the universe of equities in this data domain.
        /// </summary>
        private static readonly List<string> EquityUniverse = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Equity Universe.json");

        /// <summary>
        /// Dictionary for Accounts.
        /// </summary>
        private readonly Dictionary<string, Account> accountDictionary = new Dictionary<string, Account>();

        /// <summary>
        /// Dictionary for Blotters.
        /// </summary>
        private readonly Dictionary<string, Blotter> blotterDictionary = new Dictionary<string, Blotter>();

        /// <summary>
        /// Dictionary for Brokers.
        /// </summary>
        private readonly Dictionary<string, Broker> brokerDictionary = new Dictionary<string, Broker>();

        /// <summary>
        /// Dictionary for Managed Accounts.
        /// </summary>
        private readonly Dictionary<string, ManagedAccount> managedAccountDictionary = new Dictionary<string, ManagedAccount>();

        /// <summary>
        /// Dictionary for Prices indexed by the external identifier.
        /// </summary>
        private readonly Dictionary<string, Price> priceExternalIdDictionary = new Dictionary<string, Price>();

        /// <summary>
        /// Dictionary for Prices indexed by the FIGI code.
        /// </summary>
        private readonly Dictionary<string, Price> priceFigiDictionary = new Dictionary<string, Price>();

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Dictionary for Securities indexed by the external identifier.
        /// </summary>
        private readonly Dictionary<string, Security> securityExternalIdDictionary = new Dictionary<string, Security>();

        /// <summary>
        /// Dictionary for Securities indexed by the FIGI code.
        /// </summary>
        private readonly Dictionary<string, Security> securityFigiDictionary = new Dictionary<string, Security>();

        /// <summary>
        /// Dictionary for Security Lists.
        /// </summary>
        private readonly Dictionary<string, SecurityList> securityListDictionary = new Dictionary<string, SecurityList>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class.
        /// </summary>
        /// <param name="repository">The web service repository.</param>
        public Domain(IRepository repository)
        {
            // Initialize the object.
            this.repository = repository;

            // Create the dictionaries.
            Task task = Task.Run(this.InitializeData);
        }

        /// <summary>
        /// Gets an array of all the accounts.
        /// </summary>
        public IEnumerable<Account> Accounts
        {
            get
            {
                return new ReadOnlyCollection<Account>(this.accountDictionary.Values.ToArray());
            }
        }

        /// <summary>
        /// Gets an array of all the blotters.
        /// </summary>
        public IEnumerable<Blotter> Blotters
        {
            get
            {
                return new ReadOnlyCollection<Blotter>(this.blotterDictionary.Values.ToArray());
            }
        }

        /// <summary>
        /// Gets an array of all the brokers.
        /// </summary>
        public IEnumerable<Broker> Brokers
        {
            get
            {
                return new ReadOnlyCollection<Broker>(this.brokerDictionary.Values.ToArray());
            }
        }

        /// <summary>
        /// Gets an event that can be used to wait until the object is initialized.
        /// </summary>
        public ManualResetEvent Initialized { get; } = new ManualResetEvent(false);

        /// <summary>
        /// Gets an array of all the accounts.
        /// </summary>
        public IEnumerable<ManagedAccount> ManagedAccounts
        {
            get
            {
                return new ReadOnlyCollection<ManagedAccount>(this.managedAccountDictionary.Values.ToArray());
            }
        }

        /// <summary>
        /// Gets an array of all the securities.
        /// </summary>
        public IEnumerable<Security> Securities
        {
            get
            {
                return new ReadOnlyCollection<Security>(this.securityFigiDictionary.Values.ToArray());
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources and suppress finalization.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Find the symbol for the blotter into the unique index value.
        /// </summary>
        /// <param name="mnemonic">The external identifier of the blotter.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Account FindAccount(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the account based on the mnemonic.
            if (this.accountDictionary.TryGetValue(mnemonic, out Account account))
            {
                return account;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the blotter into the unique index value.
        /// </summary>
        /// <param name="mnemonic">The external identifier of the blotter.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Broker FindBroker(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the broker based on the symbol.
            if (this.brokerDictionary.TryGetValue(mnemonic, out Broker broker))
            {
                return broker;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the blotter into the unique index value.
        /// </summary>
        /// <param name="mnemonic">The external identifier of the blotter.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Blotter FindBlotter(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the blotter based on the external mnemonic.
            if (this.blotterDictionary.TryGetValue(mnemonic, out Blotter blotter))
            {
                return blotter;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the blotter into the unique index value.
        /// </summary>
        /// <param name="mnemonic">The external identifier of the blotter.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public ManagedAccount FindManagedAccount(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external blotter mnemonic to the primary key index value.
            if (this.managedAccountDictionary.TryGetValue(mnemonic, out ManagedAccount managedAccount))
            {
                return managedAccount;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the price into the unique index value.
        /// </summary>
        /// <param name="figi">The external FIGI code for the price.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Price FindPriceByFigi(string figi)
        {
            // Validate the argument.
            if (figi == null)
            {
                throw new ArgumentNullException(nameof(figi));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external price mnemonic to the primary key index value.
            if (this.priceFigiDictionary.TryGetValue(figi, out Price price))
            {
                return price;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the price into the unique index value.
        /// </summary>
        /// <param name="externalId">The external FIGI code for the price.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Price FindPriceByExternalId(string externalId)
        {
            // Validate the argument.
            if (externalId == null)
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external price mnemonic to the primary key index value.
            if (this.priceExternalIdDictionary.TryGetValue(externalId, out Price price))
            {
                return price;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the security using the FIGI code.
        /// </summary>
        /// <param name="figi">The FIGI code for the security.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Security FindSecurityByFigi(string figi)
        {
            // Validate the argument.
            if (figi == null)
            {
                throw new ArgumentNullException(nameof(figi));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external security mnemonic to the primary key index value.
            if (this.securityFigiDictionary.TryGetValue(figi, out Security security))
            {
                return security;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the security using the external identifier.
        /// </summary>
        /// <param name="externalId">The unique external identifier of the security.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Security FindSecurityByExternalId(string externalId)
        {
            // Validate the argument.
            if (externalId == null)
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external security mnemonic to the primary key index value.
            if (this.securityExternalIdDictionary.TryGetValue(externalId, out Security security))
            {
                return security;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the blotter into the unique index value.
        /// </summary>
        /// <param name="mnemonic">The external identifier of the blotter.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public SecurityList FindSecurityList(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external blotter mnemonic to the primary key index value.
            if (this.securityListDictionary.TryGetValue(mnemonic, out SecurityList securityList))
            {
                return securityList;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Dispose of the managed resources.
        /// </summary>
        /// <param name="disposing">An indication whether the managed resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Dispose of the managed resources.
            if (disposing)
            {
                this.Initialized.Dispose();
            }
        }

        /// <summary>
        /// Initialize the data used by this view model to create a block order.
        /// </summary>
        private async void InitializeData()
        {
            while (true)
            {
                // Get the data for this domain from the repository.
                var accounts = this.repository.GetAccountsAsync();
                var blotters = this.repository.GetBlottersAsync();
                var brokers = this.repository.GetBrokersAsync();
                var managedAccounts = this.repository.GetManagedAccountsAsync();
                var prices = this.GetPricesAsync();
                var securities = this.GetSecuritiesAsync();
                var securityLists = this.repository.GetSecurityListAsync();
                await Task.WhenAll(accounts, blotters, brokers, managedAccounts, prices, securities, securityLists).ConfigureAwait(false);

                // If any of the result sets had an error, then give it a polite pause and continue looping until we get all the data we need to initialize.
                if (accounts.Result == null || blotters.Result == null || managedAccounts.Result == null || prices.Result == null || securities.Result == null)
                {
                    await Task.Delay(Domain.RetryTime).ConfigureAwait(false);
                }

                // Take the results and place them in dictionaries.  The general idea is to take the external identifiers found in the scripted data
                // and turn them into actual internal identifiers so that when we call the API to create the orders, we don't waste the server's time
                // doing the translation.
                accounts.Result.ToList().ForEach(a => this.accountDictionary.Add(a.Mnemonic, a));
                blotters.Result.ToList().ForEach(b => this.blotterDictionary.Add(b.Mnemonic, b));
                brokers.Result.ToList().ForEach(b => this.brokerDictionary.Add(b.Symbol, b));
                managedAccounts.Result.ToList().ForEach(ma => this.managedAccountDictionary.Add(ma.Mnemonic, ma));
                prices.Result.Where(p => p.ExternalId != null).ToList().ForEach(p => this.priceExternalIdDictionary.Add(p.ExternalId, p));
                prices.Result.Where(p => p.Figi != null).ToList().ForEach(p => this.priceFigiDictionary.Add(p.Figi, p));
                securities.Result.Where(s => s.ExternalId != null).ToList().ForEach(s => this.securityExternalIdDictionary.Add(s.ExternalId, s));
                securities.Result.Where(s => s.Figi != null).ToList().ForEach(s => this.securityFigiDictionary.Add(s.Figi, s));
                securityLists.Result.ToList().ForEach(sl => this.securityListDictionary.Add(sl.Mnemonic, sl));

                // This signals that the data domain is initialized.
                this.Initialized.Set();

                // Exit the loop.
                break;
            }
        }

        /// <summary>
        /// Gets the universe of prices in this domain.
        /// </summary>
        /// <returns>A collection of just the prices used in this domain.</returns>
        private async Task<List<Price>> GetPricesAsync()
        {
            // The semaphore allows multiple parallel queries.
            List<Price> prices = new List<Price>();
            using (SemaphoreSlim resultSemaphore = new SemaphoreSlim(1))
            {
                // Start running a parallel task to add every tax lot in the list.
                var tasks = new List<Task>();
                foreach (string figi in Domain.EquityUniverse)
                {
                    // Send one or more of the allocations to thes service.  This is done in parallel, so we'll wait for all of them to finish
                    // before existing from this method.
                    tasks.Add(
                        Task.Run(
                            async () =>
                            {
                                Price security = await this.repository.GetPriceByFigiAsync(figi).ConfigureAwait(false);
                                await resultSemaphore.WaitAsync().ConfigureAwait(false);
                                prices.Add(security);
                                resultSemaphore.Release();
                            }));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);
            }

            return prices;
        }

        /// <summary>
        /// Gets the universe of securities in this domain.
        /// </summary>
        /// <returns>A collection of just the securities used in this domain.</returns>
        private async Task<List<Security>> GetSecuritiesAsync()
        {
            // This is the list of securities we're going to collect.
            List<Security> securities = new List<Security>();

            // The semaphore allows multiple parallel queries.
            using (SemaphoreSlim resultSemaphore = new SemaphoreSlim(1))
            {
                // This holds all the parallel tasks we're going to spawn.
                var tasks = new List<Task>();

                // Get the equities in our universe.
                foreach (string figi in Domain.EquityUniverse)
                {
                    // A task is spun for every equity we want to retrieve.
                    tasks.Add(
                        Task.Run(
                            async () =>
                            {
                                Security security = await this.repository.GetSecurityByFigiAsync(figi).ConfigureAwait(false);
                                await resultSemaphore.WaitAsync().ConfigureAwait(false);
                                securities.Add(security);
                                resultSemaphore.Release();
                            }));
                }

                // Get the currencies in our universe.
                foreach (string symbol in Domain.CurrencyUniverse)
                {
                    // A task is spun for every currency we want to retrieve.
                    tasks.Add(
                        Task.Run(
                            async () =>
                            {
                                Security security = await this.repository.GetSecurityByExternalIdAsync(symbol).ConfigureAwait(false);
                                await resultSemaphore.WaitAsync().ConfigureAwait(false);
                                securities.Add(security);
                                resultSemaphore.Release();
                            }));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);
            }

            return securities;
        }
    }
}