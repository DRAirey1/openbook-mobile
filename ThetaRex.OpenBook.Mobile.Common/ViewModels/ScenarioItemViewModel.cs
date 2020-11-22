// <copyright file="ScenarioItemViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThetaRex.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// View model for an item.
    /// </summary>
    public class ScenarioItemViewModel : ViewModel
    {
        /// <summary>
        /// Text for an active item.
        /// </summary>
        private string activeLabel;

        /// <summary>
        /// The description of the item.
        /// </summary>
        private string description;

        /// <summary>
        /// Text for an inactive item.
        /// </summary>
        private string inactiveLabel;

        /// <summary>
        /// An indication of whether the scenario is active or not.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// An indication of whether the display item is enabled or not.
        /// </summary>
        private bool isEnabled = true;

        /// <summary>
        /// The primary text that appears in the list box.
        /// </summary>
        private string label;

        /// <summary>
        /// Gets or sets the handler for the active scenario (usually a clear operation).
        /// </summary>
        public Func<ScenarioItemViewModel, Task> ActiveHandler { get; set; }

        /// <summary>
        /// Gets or sets the handler for the inactive scenario (ususally a set operation).
        /// </summary>
        public Func<ScenarioItemViewModel, Task> InactiveHandler { get; set; }

        /// <summary>
        /// Gets or sets the active text.
        /// </summary>
        public string ActiveLabel
        {
            get
            {
                return this.activeLabel;
            }

            set
            {
                this.SetProperty(ref this.activeLabel, value, nameof(this.ActiveLabel));
            }
        }

        /// <summary>
        /// Gets or sets command that is executed when the item is pressed.
        /// </summary>
        public Command Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter that is passed when the command is executed.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the data associated with this item.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets a short description of what the item does.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, nameof(this.Description));
            }
        }

        /// <summary>
        /// Gets or sets the inactive text.
        /// </summary>
        public string InactiveLabel
        {
            get
            {
                return this.inactiveLabel;
            }

            set
            {
                this.SetProperty(ref this.inactiveLabel, value, nameof(this.InactiveLabel));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the scenario is active or not.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.SetProperty(ref this.isActive, value, nameof(this.IsActive));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the scenario is active or not.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.SetProperty(ref this.isEnabled, value, nameof(this.IsEnabled));
            }
        }

        /// <summary>
        /// Gets the display text on the item.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }

            private set
            {
                this.SetProperty(ref this.label, value, nameof(this.Label));
            }
        }

        /// <summary>
        /// Gets or sets the scenario associated with this view model.
        /// </summary>
        public Scenario Scenario { get; set; }

        /// <inheritdoc/>
        protected override void SetProperty<T>(ref T backingStore, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                backingStore = value;
                this.OnPropertyChanged(propertyName);
                if (propertyName == nameof(this.IsActive) || propertyName == nameof(this.ActiveLabel) || propertyName == nameof(this.InactiveLabel))
                {
                    this.Label = this.IsActive ? this.ActiveLabel : this.InactiveLabel;
                }
            }
        }
    }
}