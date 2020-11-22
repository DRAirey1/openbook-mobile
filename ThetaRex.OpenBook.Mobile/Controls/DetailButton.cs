// <copyright file="DetailButton.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Controls
{
    using System;
    using System.Windows.Input;
    using Xamarin.Forms;

    /// <summary>
    /// A button with an extra line for the description.
    /// </summary>
    public class DetailButton : ContentView
    {
        /// <summary>
        /// The Command property.
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(DetailButton));

        /// <summary>
        /// The Command property.
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(DetailButton));

        /// <summary>
        /// The Description property.
        /// </summary>
        public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(DetailButton));

        /// <summary>
        /// The Label property.
        /// </summary>
        public static readonly BindableProperty LabelProperty = BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(DetailButton));

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailButton"/> class.
        /// </summary>
        public DetailButton()
        {
            // Allow this control to recognize a tap.
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += this.OnTapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);
        }

        /// <summary>
        /// Gets or sets the Command property.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(DetailButton.CommandProperty);
            set => this.SetValue(DetailButton.CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the Command property.
        /// </summary>
        public object CommandParameter
        {
            get => this.GetValue(DetailButton.CommandParameterProperty);
            set => this.SetValue(DetailButton.CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the Description property.
        /// </summary>
        public string Description
        {
            get => this.GetValue(DetailButton.DescriptionProperty) as string;
            set => this.SetValue(DetailButton.DescriptionProperty, value);
        }

        /// <summary>
        /// Gets or sets the Label property.
        /// </summary>
        public string Label
        {
            get => this.GetValue(DetailButton.LabelProperty) as string;
            set => this.SetValue(DetailButton.LabelProperty, value);
        }

        /// <summary>
        /// Handles a change to the Command property.
        /// </summary>
        /// <param name="bindable">The bindable object that contains the property.</param>
        /// <param name="oldValue">The old property value.</param>
        /// <param name="newValue">The new property value.</param>
        private static void OnCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            DetailButton commandCell = bindable as DetailButton;
            ICommand newCommand = newValue as ICommand;
            ICommand oldCommand = oldValue as ICommand;
            if (newValue == null)
            {
                oldCommand.CanExecuteChanged -= commandCell.OnCanExecuteChanged;
            }
            else
            {
                newCommand.CanExecuteChanged += commandCell.OnCanExecuteChanged;
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.IsEnabled = false;
        }

        /// <summary>
        /// Handles the tapped event.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnTapped(object sender, EventArgs eventArgs)
        {
            // When the control is tapped, execute the command, just like a button.
            if (this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}