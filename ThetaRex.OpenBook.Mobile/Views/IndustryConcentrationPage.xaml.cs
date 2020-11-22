// <copyright file="IndustryConcentrationPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// The single issue scenarios page.
    /// </summary>
    public partial class IndustryConcentrationPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndustryConcentrationPage"/> class.
        /// </summary>
        /// <param name="industryConcentrationViewModel">The view model.</param>
        public IndustryConcentrationPage(IndustryConcentrationViewModel industryConcentrationViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = industryConcentrationViewModel;
        }
    }
}