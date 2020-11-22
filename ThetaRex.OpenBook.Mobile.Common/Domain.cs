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
    using System.Threading;
    using System.Threading.Tasks;

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
        /// Dictionary for Accounts.
        /// </summary>
        private readonly Dictionary<string, Account> accountDictionary = new Dictionary<string, Account>();

        /// <summary>
        /// Dictionary for Blotters.
        /// </summary>
        private readonly Dictionary<string, Blotter> blotterDictionary = new Dictionary<string, Blotter>();

        /// <summary>
        /// Dictionary for Managed Accounts.
        /// </summary>
        private readonly Dictionary<string, ManagedAccount> managedAccountDictionary = new Dictionary<string, ManagedAccount>();

        /// <summary>
        /// Dictionary for Prices.
        /// </summary>
        private readonly Dictionary<string, Price> priceDictionary = new Dictionary<string, Price>();

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Dictionary for Securities.
        /// </summary>
        private readonly Dictionary<string, Security> securityDictionary = new Dictionary<string, Security>();

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
                return new ReadOnlyCollection<Security>(this.securityDictionary.Values.ToArray());
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

            // Find the external blotter mnemonic to the primary key index value.
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
        public Blotter FindBlotter(string mnemonic)
        {
            // Validate the argument.
            if (mnemonic == null)
            {
                throw new ArgumentNullException(nameof(mnemonic));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external blotter mnemonic to the primary key index value.
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
        public Price FindPrice(string figi)
        {
            // Validate the argument.
            if (figi == null)
            {
                throw new ArgumentNullException(nameof(figi));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external price mnemonic to the primary key index value.
            if (this.priceDictionary.TryGetValue(figi, out Price price))
            {
                return price;
            }

            // This indicates the record can't be found with the given external identifier.
            return null;
        }

        /// <summary>
        /// Find the symbol for the security into the unique index value.
        /// </summary>
        /// <param name="figi">The external FIGI code for the security.</param>
        /// <returns>The internal primary key value for the record with the given external identifier, or null if the value can't be translated.</returns>
        public Security FindSecurity(string figi)
        {
            // Validate the argument.
            if (figi == null)
            {
                throw new ArgumentNullException(nameof(figi));
            }

            // Wait for the object to be initialized.
            this.Initialized.WaitOne();

            // Find the external security mnemonic to the primary key index value.
            if (this.securityDictionary.TryGetValue(figi, out Security security))
            {
                return security;
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
                // Start the tasks to get the accounts, blotters and securities.  We're going to build a dictionary that translates the external
                // identifiers into internal primary index values.
                var accountsTask = this.repository.GetAccountsAsync();
                var blottersTask = this.repository.GetBlottersAsync();
                var managedAccountsTask = this.repository.GetManagedAccountsAsync();
                var pricesTask = this.repository.GetPricesAsync();
                var securitiesTask = this.repository.GetSecuritiesAsync();

                // Wait for the results to come back.
                var accounts = await accountsTask.ConfigureAwait(true);
                var blotters = await blottersTask.ConfigureAwait(true);
                var managedAccounts = await managedAccountsTask.ConfigureAwait(true);
                var prices = await pricesTask.ConfigureAwait(true);
                var securities = await securitiesTask.ConfigureAwait(true);

                // If successful, digest the web service data into an internal database.
                if (accounts != null && blotters != null && managedAccounts != null && prices != null && securities != null)
                {
                    // Take the results and place them in dictionaries.  The general idea is to take the external identifiers found in the scripted data and
                    // turn them into actual internal identifiers so that when we call the API to create the block order, we don't waste the server's time
                    // doing the translation.
                    accounts.ToList().ForEach(a => this.accountDictionary.Add(a.Mnemonic, a));
                    blotters.ToList().ForEach(b => this.blotterDictionary.Add(b.Mnemonic, b));
                    managedAccounts.ToList().ForEach(ma => this.managedAccountDictionary.Add(ma.Mnemonic, ma));
                    prices.Where(p => p.Figi != null).ToList().ForEach(p => this.priceDictionary.Add(p.Figi, p));
                    securities.Where(p => p.Figi != null).ToList().ForEach(s => this.securityDictionary.Add(s.Figi, s));

                    // Signal that the object is initialized.
                    this.Initialized.Set();

                    // Exit the loop.
                    break;
                }

                // Wait before trying this again.
                await Task.Delay(Domain.RetryTime).ConfigureAwait(false);
            }
        }
    }
}