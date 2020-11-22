// <copyright file="SingleAccountPage.xaml.cs" company="Theta Rex, Inc.">
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
    public partial class SingleAccountPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAccountPage"/> class.
        /// </summary>
        /// <param name="singleAccountViewModel">The view model.</param>
        public SingleAccountPage(SingleAccountViewModel singleAccountViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = singleAccountViewModel;
        }
    }
}