// <copyright file="BulkAccountPage.xaml.cs" company="Theta Rex, Inc.">
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
    public partial class BulkAccountPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkAccountPage"/> class.
        /// </summary>
        /// <param name="bulkAccountViewModel">The view model.</param>
        public BulkAccountPage(BulkAccountViewModel bulkAccountViewModel)
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();

            // Provide a view model for the page.
            this.BindingContext = bulkAccountViewModel;
        }
    }
}