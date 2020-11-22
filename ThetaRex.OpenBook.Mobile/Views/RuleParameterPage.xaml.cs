// <copyright file="RuleParameterPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// Bulk operations scenario page.
    /// </summary>
    public partial class RuleParameterPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleParameterPage"/> class.
        /// </summary>
        /// <param name="ruleParameterViewModel">The view model.</param>
        public RuleParameterPage(RuleParameterViewModel ruleParameterViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = ruleParameterViewModel;
        }
    }
}