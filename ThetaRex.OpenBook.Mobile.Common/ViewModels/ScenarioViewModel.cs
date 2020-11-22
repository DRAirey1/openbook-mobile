// <copyright file="ScenarioViewModel.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using ThetaRex.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// Scenarios for changing rule parameters.
    /// </summary>
    public class ScenarioViewModel : ViewModel
    {
        /// <summary>
        /// The title of the view.
        /// </summary>
        private string title;

        /// <summary>
        /// Gets the scenarios in the list view.
        /// </summary>
        public ScenarioCollection Items { get; } = new ScenarioCollection();

        /// <summary>
        /// Gets the command that is executed when the item is pressed.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public Command Command { get; }

        /// <summary>
        /// Gets the command parameter that is passed when the command is executed.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public object CommandParameter { get; }

        /// <summary>
        /// Gets a short description of what the item does.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether the scenario is active or not.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets the label on the item.
        /// </summary>
        /// <remarks>[TODO] This is a dummy to prevent debug messages related to binding.  Remove when the bug is fixed.</remarks>
        public string Label { get; }

        /// <summary>
        /// Gets or sets the title of the view.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value, nameof(this.Title)); }
        }

        /// <summary>
        /// Routes the command to the given handler.
        /// </summary>
        /// <param name="scenario">The scenario that was selected.</param>
        protected void RouteCommand(Scenario scenario)
        {
            // The routing of any command shouldn't kill the application.  This is where any serious exception handling shoud be for the commands.
            try
            {
                // The basic idea here is that you can either set or reset a scenario.  The state of the 'IsActive' flag determines whether we're setting
                // the scenario or clearing it.
                ScenarioItemViewModel scenarioItemViewModel = this.Items[scenario];
                if (scenarioItemViewModel.IsActive)
                {
                    scenarioItemViewModel.ActiveHandler(scenarioItemViewModel);
                }
                else
                {
                    scenarioItemViewModel.InactiveHandler(scenarioItemViewModel);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            finally
            {
            }
        }
    }
}