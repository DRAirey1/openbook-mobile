// <copyright file="App.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile
{
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using ThetaRex.OpenBook.Mobile.Repository;
    using ThetaRex.OpenBook.Mobile.Views;
    using Xamarin.Forms;

    /// <summary>
    /// The application.
    /// </summary>
    public partial class App : Application
    {
#if PRODUCTION
        private const string EnvironmentName = "Production";
#elif STAGING
        private const string EnvironmentName = "Staging";
#elif DEVELOPMENT
        private const string EnvironmentName = "Development";
#else
        private const string EnvironmentName = "Local";
#endif

        /// <summary>
        /// The Dependency Injection container.
        /// </summary>
        private readonly ServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="packageInfo">Information about the application package.</param>
        public App(PackageInfo packageInfo)
        {
            // Initializes the IDE maintained components.
            this.InitializeComponent();

            // Build the configuration.  The configuration files are embedded in this assembly.  This allows us to not have to worry about the
            // differences between the file systems on the different devices or where the 'Content' files might end up.
            var assembly = Assembly.GetExecutingAssembly();
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .AddJsonStream(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json"))
               .AddJsonStream(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.{App.EnvironmentName}.json"))
               .Build();

            // Build the Dependency Injection container.
            this.serviceProvider = new ServiceCollection()
                .AddLocalization()
                .UseThetaRexCommon(configuration)
                .UseThetaRexOpenBookMobileViews()
                .UseThetaRexOpenBookMobileViewModels()
                .AddSingleton<IRepository, OpenBookRepository>()
                .AddSingleton(packageInfo)
                .AddSingleton<User>()
                .AddSingleton<HttpClient<OpenBookHost>>()
                .BuildServiceProvider();
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            // This is now the main page.
            this.MainPage = this.serviceProvider.GetRequiredService<MainPage>();

            // Allow the base class to finish starting the app.
            base.OnStart();
        }
    }
}