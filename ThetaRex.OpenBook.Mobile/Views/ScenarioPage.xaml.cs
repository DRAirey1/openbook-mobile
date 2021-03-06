﻿// <copyright file="ScenarioPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// Page for selecting the scenario group (single issue scenarios, bulk operation scenarios, etc.)
    /// </summary>
    public partial class ScenarioPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioPage"/> class.
        /// </summary>
        /// <param name="scenarioViewModel">The scenario selection view model.</param>
        public ScenarioPage(RootViewModel scenarioViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = scenarioViewModel;
        }
    }
}