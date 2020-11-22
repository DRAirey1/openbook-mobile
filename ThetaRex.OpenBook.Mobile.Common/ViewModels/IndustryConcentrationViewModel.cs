// <copyright file="IndustryConcentrationViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Industry Concentration Scenarios.
    /// </summary>
    public class IndustryConcentrationViewModel : ScenarioViewModel
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
        /// Initializes a new instance of the <see cref="IndustryConcentrationViewModel"/> class.
        /// </summary>
        /// <param name="repository">The data repository.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public IndustryConcentrationViewModel(IRepository repository, IStringLocalizer<IndustryConcentrationViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.repository = repository;
            this.stringLocalizer = stringLocalizer;

            // Localize the object.
            this.Title = this.stringLocalizer["Title"];

            // Listen for the reset signal from the main page.
            MessagingCenter.Subscribe(
                this,
                MessengerKeys.ResetScenario,
                async (RootViewModel scenarioSelectionViewModel) => await this.SetConcentration(0.14m).ConfigureAwait(true));

            // Add the items that invoke the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.SetConcentration(0.14m).ConfigureAwait(true)),
                    CommandParameter = Scenario.Reset,
                    Description = this.stringLocalizer["ResetScenariosDescription"],
                    InactiveLabel = this.stringLocalizer["ResetScenariosInactiveLabel"],
                    Scenario = Scenario.Reset,
                });
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.SetConcentration(0.11m).ConfigureAwait(true)),
                    CommandParameter = Scenario.SetConcentrationTo11,
                    Description = this.stringLocalizer["IndustryConcentration11Description"],
                    InactiveLabel = this.stringLocalizer["IndustryConcentration11InactiveLabel"],
                    Scenario = Scenario.SetConcentrationTo11,
                });
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.SetConcentration(0.12m).ConfigureAwait(true)),
                    CommandParameter = Scenario.SetConcentrationTo12,
                    Description = this.stringLocalizer["IndustryConcentration12Description"],
                    InactiveLabel = this.stringLocalizer["IndustryConcentration12InactiveLabel"],
                    Scenario = Scenario.SetConcentrationTo12,
                });
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.SetConcentration(0.13m).ConfigureAwait(true)),
                    CommandParameter = Scenario.SetConcentrationTo13,
                    Description = this.stringLocalizer["IndustryConcentration13Description"],
                    InactiveLabel = this.stringLocalizer["IndustryConcentration13InactiveLabel"],
                    Scenario = Scenario.SetConcentrationTo13,
                });
        }

        /// <summary>
        /// Set the Retail Trade benchmark.
        /// </summary>
        /// <param name="value">The new benchmark value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task SetConcentration(decimal value)
        {
            // Disable all commands while resetting.
            this.Items[Scenario.Reset].IsEnabled = false;
            this.Items[Scenario.SetConcentrationTo11].IsEnabled = false;
            this.Items[Scenario.SetConcentrationTo12].IsEnabled = false;
            this.Items[Scenario.SetConcentrationTo13].IsEnabled = false;

            // Set the Retail Trade concentration to the given fraction.
            var categoryBenchmark = await this.repository.GetCategoryBenchmarkByCategoryBenchmarkExternalKeyAsync("DOWSICSIC.52").ConfigureAwait(true);
            categoryBenchmark.Weight = value;
            await this.repository.UpdateClassificationBenchmarkAsync(new CategoryBenchmark[] { categoryBenchmark }).ConfigureAwait(true);

            // re-enable the commands after the reset.
            this.Items[Scenario.Reset].IsEnabled = true;
            this.Items[Scenario.SetConcentrationTo11].IsEnabled = true;
            this.Items[Scenario.SetConcentrationTo12].IsEnabled = true;
            this.Items[Scenario.SetConcentrationTo13].IsEnabled = true;
        }
    }
}