// <copyright file="RuleParameterViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using Microsoft.Extensions.Localization;
    using ThetaRex.OpenBook.Mobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios for changing rule parameters.
    /// </summary>
    public class RuleParameterViewModel : ScenarioViewModel
    {
        /// <summary>
        /// The navigation interface for selecting scenarios.
        /// </summary>
        private readonly PageNavigation pageNavigation;

        /// <summary>
        /// The string localizer.
        /// </summary>
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleParameterViewModel"/> class.
        /// </summary>
        /// <param name="pageNavigation">The view model based navigation.</param>
        /// <param name="stringLocalizer">The string localizer.</param>
        public RuleParameterViewModel(PageNavigation pageNavigation, IStringLocalizer<RuleParameterViewModel> stringLocalizer)
        {
            // Initialize the object.
            this.pageNavigation = pageNavigation;
            this.stringLocalizer = stringLocalizer;

            // Localize the object.
            this.Title = this.stringLocalizer["Title"];

            // Add the items that invoke the scenarios.
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.pageNavigation.PushAsync(typeof(IndustryConcentrationViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["IndustryConcentrationDescription"],
                    InactiveLabel = this.stringLocalizer["IndustryConcentrationInactiveLabel"],
                });
            this.Items.Add(
                new ScenarioItemViewModel
                {
                    Command = new Command(async () => await this.pageNavigation.PushAsync(typeof(RestrictedListViewModel)).ConfigureAwait(true)),
                    Description = this.stringLocalizer["RestrictedListDescription"],
                    InactiveLabel = this.stringLocalizer["RestrictedListInactiveLabel"],
                });
        }
    }
}