// <copyright file="ServiceCollectionExtensions.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Common
{
    using Microsoft.Extensions.DependencyInjection;
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;

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
        public static IServiceCollection UseThetaRexOpenBookMobileViewModels(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<Domain>()
                .AddSingleton<AboutViewModel>()
                .AddSingleton<BulkAccountViewModel>()
                .AddSingleton<IborViewModel>()
                .AddSingleton<IndustryConcentrationViewModel>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<RestrictedListViewModel>()
                .AddSingleton<RuleParameterViewModel>()
                .AddSingleton<RootViewModel>()
                .AddSingleton<SingleAccountViewModel>()
                .AddSingleton<TradingViewModel>();
        }
    }
}