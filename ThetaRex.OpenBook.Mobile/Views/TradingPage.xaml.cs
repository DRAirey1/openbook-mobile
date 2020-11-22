// <copyright file="TradingPage.xaml.cs" company="Theta Rex, Inc.">
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
    public partial class TradingPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradingPage"/> class.
        /// </summary>
        /// <param name="tradingViewModel">The view model.</param>
        public TradingPage(TradingViewModel tradingViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = tradingViewModel;
        }
    }
}