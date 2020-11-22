// <copyright file="MenuItemViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System.Windows.Input;
    using ThetaRex.Common.ViewModels;

    /// <summary>
    /// View model for an item.
    /// </summary>
    public class MenuItemViewModel : ViewModel
    {
        /// <summary>
        /// The image that appears in the manu item.
        /// </summary>
        private string image;

        /// <summary>
        /// The text that appears in the menu item.
        /// </summary>
        private string label;

        /// <summary>
        /// Gets or sets command that is executed when the item is pressed.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets command that is executed when the item is pressed.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the image on the menu item.
        /// </summary>
        public string Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.SetProperty(ref this.image, value, nameof(this.Image));
            }
        }

        /// <summary>
        /// Gets or sets the label on the menu item.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }

            set
            {
                this.SetProperty(ref this.label, value, nameof(this.Label));
            }
        }
    }
}