// <copyright file="CurrencySlider.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Controls
{
    using System;
    using Xamarin.Forms;

    /// <summary>
    /// A View control that inputs a currency value.
    /// </summary>
    public class CurrencySlider : Slider
    {
        /// <summary>
        /// The Maximum property.
        /// </summary>
        public static new readonly BindableProperty MaximumProperty = BindableProperty.Create(
            nameof(CurrencySlider.Maximum),
            typeof(decimal),
            typeof(CurrencySlider),
            propertyChanged: (bo, o, n) => (bo as Slider).Maximum = Convert.ToDouble((decimal)n));

        /// <summary>
        /// The Minimum property.
        /// </summary>
        public static new readonly BindableProperty MinimumProperty = BindableProperty.Create(
            nameof(CurrencySlider.Minimum),
            typeof(decimal),
            typeof(CurrencySlider),
            propertyChanged: (bo, o, n) => (bo as Slider).Minimum = Convert.ToDouble((decimal)n));

        /// <summary>
        /// The Value property.
        /// </summary>
        public static new readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(CurrencySlider.Value),
            typeof(decimal),
            typeof(CurrencySlider),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bo, o, n) => (bo as Slider).Value = Convert.ToDouble((decimal)n));

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencySlider"/> class.
        /// </summary>
        public CurrencySlider()
        {
            // Caputure
            this.ValueChanged += (o, e) => this.Value = Convert.ToDecimal(e.NewValue);
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public new decimal Maximum
        {
            get => (decimal)this.GetValue(CurrencySlider.MaximumProperty);
            set => this.SetValue(CurrencySlider.MaximumProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public new decimal Minimum
        {
            get => (decimal)this.GetValue(CurrencySlider.MinimumProperty);
            set => this.SetValue(CurrencySlider.MinimumProperty, value);
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public new decimal Value
        {
            get => (decimal)this.GetValue(CurrencySlider.ValueProperty);
            set => this.SetValue(CurrencySlider.ValueProperty, value);
        }
    }
}