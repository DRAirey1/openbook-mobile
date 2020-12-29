// <copyright file="ServiceCollectionExtensions.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile
{
    using Microsoft.Extensions.DependencyInjection;
    using ThetaRex.OpenBook.Mobile.Common;
    using ThetaRex.OpenBook.Mobile.Views;

    /// <summary>
    /// Used to add the services found in this library.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the Dependency Injection container to reference the classes in this library.
        /// </summary>
        /// <param name="serviceCollection">The DI container.</param>
        /// <returns>The same container.</returns>
        public static IServiceCollection UseThetaRexOpenBookMobileViews(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<AboutPage>()
                .AddSingleton<App>()
                .AddSingleton<BulkAccountPage>()
                .AddSingleton<ChangePricePage>()
                .AddSingleton<IborPage>()
                .AddSingleton<IndustryConcentrationPage>()
                .AddSingleton<MainPage>()
                .AddSingleton<MasterPage>()
                .AddSingleton<Navigator>()
                .AddSingleton<RestrictedListPage>()
                .AddSingleton<RuleParameterPage>()
                .AddSingleton<ScenarioPage>()
                .AddSingleton<SingleAccountPage>()
                .AddSingleton<TradingPage>();
        }
    }
}