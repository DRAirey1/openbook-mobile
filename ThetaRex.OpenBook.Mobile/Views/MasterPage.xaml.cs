// <copyright file="MasterPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// The Master Page.
    /// </summary>
    public partial class MasterPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPage"/> class.
        /// </summary>
        /// <param name="mainViewModel">The view model for the main page.</param>
        public MasterPage(MainViewModel mainViewModel)
        {
            // Initialize the object.
            this.BindingContext = mainViewModel;

            // Initialize the IDE managed components.
            this.InitializeComponent();
        }
    }
}