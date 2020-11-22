// <copyright file="ScenarioCollection.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// An observable collection of <see cref="ScenarioItemViewModel"/>s.
    /// </summary>
    public class ScenarioCollection : ObservableCollection<ScenarioItemViewModel>
    {
        /// <summary>
        /// Indexer into an observable list of <see cref="ScenarioItemViewModel"/>s.
        /// </summary>
        /// <param name="scenario">The scenario to be returned.</param>
        /// <returns>The view model matching the given scenario.</returns>
        public ScenarioItemViewModel this[Scenario scenario]
        {
            get
            {
                // Return the view model matching the given scenario.
                return this.Where(sivm => sivm.Scenario == scenario).First();
            }
        }
    }
}
