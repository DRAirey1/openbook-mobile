// <copyright file="CurrencyControl.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Controls
{
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    /// <summary>
    /// Provides a lightweight control for displaying small amounts of flow content in a user specified format.
    /// </summary>
    public class CurrencyControl : ContentView
    {
        /// <summary>
        /// The Current property.
        /// </summary>
        public static readonly BindableProperty CurrentProperty = BindableProperty.Create(
            nameof(CurrencyControl.Current),
            typeof(decimal),
            typeof(CurrencyControl),
            propertyChanged: CurrencyControl.OnCurrentPropertyChanged);

        /// <summary>
        /// The FontSize property.
        /// </summary>
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(CurrencyControl.FontSize),
            typeof(double),
            typeof(CurrencyControl));

        /// <summary>
        /// The Duration property.
        /// </summary>
        public static readonly BindableProperty DurationProperty = BindableProperty.Create(
            nameof(CurrencyControl.Duration),
            typeof(TimeSpan),
            typeof(CurrencyControl),
            TimeSpan.FromSeconds(1.0));

        /// <summary>
        /// The Format property.
        /// </summary>
        public static readonly BindableProperty FormatProperty = BindableProperty.Create(
            nameof(CurrencyControl.Format),
            typeof(string),
            typeof(CurrencyControl),
            "#,##0.00",
            propertyChanged: CurrencyControl.OnFormatPropertyChanged);

        /// <summary>
        /// The IsUp property.
        /// </summary>
        public static readonly BindableProperty IsUpProperty = BindableProperty.Create(
            nameof(CurrencyControl.IsUp),
            typeof(bool),
            typeof(CurrencyControl));

        /// <summary>
        /// The Previous property.
        /// </summary>
        public static readonly BindableProperty PreviousProperty = BindableProperty.Create(
            nameof(CurrencyControl.Previous),
            typeof(decimal?),
            typeof(CurrencyControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyControl"/> class.
        /// </summary>
        public CurrencyControl()
        {
            // The content of this window is a label where the text version of the currency is displayed.
            Label label = new Label();
            label.BindingContext = this;
            label.SetBinding(Label.FontSizeProperty, new Binding("FontSize"));
            this.Content = label;
        }

        /// <summary>
        /// Gets or sets the current price.
        /// </summary>
        public decimal Current
        {
            get => (decimal)this.GetValue(CurrencyControl.CurrentProperty);
            set => this.SetValue(CurrencyControl.CurrentProperty, value);
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public double FontSize
        {
            get => (double)this.GetValue(CurrencyControl.FontSizeProperty);
            set => this.SetValue(CurrencyControl.FontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the format field used to display the <see cref="decimal"/>.
        /// </summary>
        public string Format
        {
            get => this.GetValue(CurrencyControl.FormatProperty) as string;
            set => this.SetValue(CurrencyControl.FormatProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value has moved up.
        /// </summary>
        public bool IsUp
        {
            get => (bool)this.GetValue(CurrencyControl.IsUpProperty);
            set => this.SetValue(CurrencyControl.IsUpProperty, value);
        }

        /// <summary>
        /// Gets or sets the previous price.
        /// </summary>
        public decimal? Previous
        {
            get => (decimal?)this.GetValue(CurrencyControl.PreviousProperty);
            set => this.SetValue(CurrencyControl.PreviousProperty, value);
        }

        /// <summary>
        /// Gets or sets the amount of time that the animation is active.
        /// </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)this.GetValue(CurrencyControl.DurationProperty);
            set => this.SetValue(CurrencyControl.DurationProperty, value);
        }

        /// <summary>
        /// Handles a change to the Price property.
        /// </summary>
        /// <param name="bindableObject">The bindable object that contains the property.</param>
        /// <param name="oldValue">The old property value.</param>
        /// <param name="newValue">The new property value.</param>
        private static void OnCurrentPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            // Format the content according to the specification in the Format property.
            CurrencyControl currencyControl = bindableObject as CurrencyControl;
            if (currencyControl.Content is Label label)
            {
                label.Text = currencyControl.Current.ToString(currencyControl.Format, CultureInfo.CurrentCulture);
            }

            // Set the mask based on whether the price moved up or down (or hasn't changed).
            currencyControl.IsUp = currencyControl.Previous < currencyControl.Current;
        }

        /// <summary>
        /// Handles a change to the Format property.
        /// </summary>
        /// <param name="bindableObject">The bindable object that contains the property.</param>
        /// <param name="oldValue">The old property value.</param>
        /// <param name="newValue">The new property value.</param>
        private static void OnFormatPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            // This will convert the value into a text string that can be displayed in the base TextBlock.
            CurrencyControl currencyControl = bindableObject as CurrencyControl;
            if (currencyControl.Content is Label label)
            {
                label.Text = currencyControl.Current.ToString(currencyControl.Format, CultureInfo.CurrentCulture);
            }
        }
    }
}