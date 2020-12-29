// <copyright file="ChangePriceViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.Extensions.Localization;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios involving a single issue.
    /// </summary>
    public class ChangePriceViewModel : ScenarioViewModel
    {
        /// <summary>
        /// The FIGIs for the single account optimization.
        /// </summary>
        private static readonly List<string> SingleAccount = ResourceHelper.ReadEmbeddedFile<List<string>>(
            Assembly.GetExecutingAssembly(),
            "ThetaRex.OpenBook.Mobile.Common.Data.Single Account.json");

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
        /// An indication whether the 'Reset All' button is enabled.
        /// </summary>
        private bool isResetAllEnabled = true;

        /// <summary>
        /// The maximum price of the equity.
        /// </summary>
        private decimal maximumPrice;

        /// <summary>
        /// The minimum price of the equity.
        /// </summary>
        private decimal minimumPrice;

        /// <summary>
        /// The price of the equity.
        /// </summary>
        private decimal price;

        /// <summary>
        /// The selected item from the ticker drop down list.
        /// </summary>
        private object selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePriceViewModel"/> class.
        /// </summary>
        /// <param name="domain">The data domain.</param>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public ChangePriceViewModel(Domain domain, IRepository repository, IStringLocalizer<ChangePriceViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.domain = domain;
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;

            // Localist the object.
            this.Title = this.stringLocalizer["Title"];

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel rootViewModel) => await this.ResetScenariosAsync().ConfigureAwait(true));

            // Handle the changing dropdown box with the tickers.
            this.PropertyChangedActions.Add("SelectedItem", this.OnSelectItemChanged);

            // We need the data domain to be initialized before we can popuate the view model.  Note that we can't access the windows controls on the
            // background thread (the UI controls are tied to events on the view model), so we need to wait for the initialization task to complete
            // before we initialize the view model with the first security in our data domain.
            Task task = Task
                .Run(this.InitializeData)
                .ContinueWith(t => { this.SelectedItem = this.Tickers[0]; }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Gets the local string for the Change button label.
        /// </summary>
        public string ChangeLabel => this.stringLocalizer["ChangeLabel"];

        /// <summary>
        /// Gets the Change price command.
        /// </summary>
        public ICommand ChangeCommand => new SimpleCommand(async o => await this.OnChangeCommand().ConfigureAwait(true));

        /// <summary>
        /// Gets or sets a value indicating whether the 'Reset All' button is enabled.
        /// </summary>
        public bool IsResetAllEnabled
        {
            get { return this.isResetAllEnabled; }
            set { this.SetProperty(ref this.isResetAllEnabled, value, nameof(this.IsResetAllEnabled)); }
        }

        /// <summary>
        /// Gets or sets the maximum price.
        /// </summary>
        public decimal MaximumPrice
        {
            get { return this.maximumPrice; }
            set { this.SetProperty(ref this.maximumPrice, value, nameof(this.MaximumPrice)); }
        }

        /// <summary>
        /// Gets or sets the minimum price.
        /// </summary>
        public decimal MinimumPrice
        {
            get { return this.minimumPrice; }
            set { this.SetProperty(ref this.minimumPrice, value, nameof(this.MinimumPrice)); }
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price
        {
            get { return this.price; }
            set { this.SetProperty(ref this.price, value, nameof(this.Price)); }
        }

        /// <summary>
        /// Gets the Change price command.
        /// </summary>
        public ICommand ResetAllCommand => new SimpleCommand(async o => await this.OnResetAllCommandAsync().ConfigureAwait(true));

        /// <summary>
        /// Gets the local string for the Reset button label.
        /// </summary>
        public string ResetAllLabel => this.stringLocalizer["ResetAllLabel"];

        /// <summary>
        /// Gets the Change price command.
        /// </summary>
        public ICommand ResetCommand => new SimpleCommand(async o => await this.ResetScenariosAsync().ConfigureAwait(true));

        /// <summary>
        /// Gets the local string for the Reset button label.
        /// </summary>
        public string ResetLabel => this.stringLocalizer["ResetLabel"];

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return this.selectedItem; }
            set { this.SetProperty(ref this.selectedItem, value, nameof(this.SelectedItem)); }
        }

        /// <summary>
        /// Gets the list of tickers.
        /// </summary>
        public ObservableCollection<Tuple<string, string>> Tickers { get; } = new ObservableCollection<Tuple<string, string>>();

        /// <summary>
        /// Initialize the data.
        /// </summary>
        private void InitializeData()
        {
            // Wait for the domain to be initialized.
            this.domain.Initialized.WaitOne();

            // Create a dictionary for the security symbols.
            List<Tuple<string, string>> rawTickers = new List<Tuple<string, string>>();
            foreach (string figi in ChangePriceViewModel.SingleAccount)
            {
                var security = this.domain.FindSecurityByFigi(figi);
                rawTickers.Add(new Tuple<string, string>(security.Figi, security.Ticker));
            }

            rawTickers.Sort((f, s) => f.Item2.CompareTo(s.Item2));
            rawTickers.ForEach(t => this.Tickers.Add(t));
        }

        /// <summary>
        /// Handles a request to change the price.
        /// </summary>
        private async Task OnChangeCommand()
        {
            // Get the item selected from the drop-down list and update the view model with the price.
            string figi = ((Tuple<string, string>)this.selectedItem).Item1;
            var oldPrice = this.domain.FindPriceByFigi(figi);
            var newPrice = await this.repository.GetPriceAsync(oldPrice.PriceId).ConfigureAwait(true);
            if (newPrice != null)
            {
                newPrice.LastPrice = this.Price;
                await this.repository.UpdatePriceAsync(new Price[] { newPrice }).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Handles a change to the selected item.
        /// </summary>
        private void OnSelectItemChanged()
        {
            // The slider control needs to be reset after every itme change because the minimum price must always be less than the maximum price and
            // the price must always be between the maximum and minimum.
            this.MinimumPrice = decimal.Zero;
            this.Price = decimal.Zero;
            this.MaximumPrice = decimal.One;

            // Set the minimum, maximum and current price.  We currently set the limits to 1.0 and 1,000.0 to provide enough range to mess with the
            // concentration levels.
            Tuple<string, string> tuple = this.SelectedItem as Tuple<string, string>;
            Price price = this.domain.FindPriceByFigi(tuple.Item1);
            this.MaximumPrice = 1000.0m;
            this.MinimumPrice = 1.0m;
            this.Price = price.LastPrice;
        }

        /// <summary>
        /// Reset all the prices.
        /// </summary>
        private async Task OnResetAllCommandAsync()
        {
            // Disable the reset button while we're executing the command.
            this.IsResetAllEnabled = false;

            // Reset all the prices.
            IEnumerable<Price> prices = await this.repository.GetPricesAsync().ConfigureAwait(false);
            foreach (Price price in prices)
            {
                price.LastPrice = price.ClosePrice;
            }

            // Update the price table so that the current price is the same as the closing price.  This resets the simulator when it drifts.
            await this.repository.UpdatePriceAsync(prices).ConfigureAwait(false);

            // Re-enable the button once complete.
            this.IsResetAllEnabled = true;
        }

        /// <summary>
        /// Reset the trading scenarios.
        /// </summary>
        private async Task ResetScenariosAsync()
        {
            // Reset the prices to the value they had when we started the application.  This is a quick way to clear
            List<Price> prices = new List<Price>();
            foreach (string figi in ChangePriceViewModel.SingleAccount)
            {
                var oldPrice = this.domain.FindPriceByFigi(figi);
                var newPrice = await this.repository.GetPriceAsync(oldPrice.PriceId).ConfigureAwait(true);
                if (newPrice != null)
                {
                    newPrice.LastPrice = oldPrice.LastPrice;
                    prices.Add(newPrice);
                }
            }

            // Update all the prices in the local scenario.
            await this.repository.UpdatePriceAsync(prices).ConfigureAwait(true);

            // Re-select the latest object to update the values.
            string selectedFigi = ((Tuple<string, string>)this.selectedItem).Item1;
            var selectedPrice = this.domain.FindPriceByFigi(selectedFigi);
            this.Price = selectedPrice.LastPrice;
        }
    }
}