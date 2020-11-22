// <copyright file="RestrictedListPage.xaml.cs" company="Theta Rex, Inc.">
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
    public partial class RestrictedListPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictedListPage"/> class.
        /// </summary>
        /// <param name="restrictedListViewModel">The view model.</param>
        public RestrictedListPage(RestrictedListViewModel restrictedListViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = restrictedListViewModel;
        }
    }
}