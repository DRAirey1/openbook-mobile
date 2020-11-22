// <copyright file="MainPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;
    using ThetaRex.OpenBook.Mobile.Common.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// Main page of the application.
    /// </summary>
    public partial class MainPage : MasterDetailPage
    {
        /// <summary>
        /// The HTTP client use to communicate with the service.
        /// </summary>
        private readonly HttpClient<OpenBookHost> httpClient;

        /// <summary>
        /// The information about the package (program name, revision, etc.)
        /// </summary>
        private readonly PackageInfo packageInfo;

        /// <summary>
        /// Provides navigation from the view model.
        /// </summary>
        private readonly PageNavigation pageNavigation;

        /// <summary>
        /// The repository for storing and retrieving data.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The view model.
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// The user's identity.
        /// </summary>
        private readonly User user;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the service.</param>
        /// <param name="mainViewModel">The view model for the main page.</param>
        /// <param name="masterPage">The view for the master in the master-detail view.</param>
        /// <param name="packageInfo">Information about the package.</param>
        /// <param name="pageNavigation">The translation between view model navigation and view navigation.</param>
        /// <param name="repository">The repository where data is read and written.</param>
        /// <param name="user">The identity of the current user.</param>
        /// <param name="aboutPage">The 'About' page.</param>
        /// <param name="bulkOperationPage">The bulk opreations scenarios page.</param>
        /// <param name="iborPage">The IBOR scenario page.</param>
        /// <param name="industryConcentrationPage">The industry concentration scenario page.</param>
        /// <param name="scenarioPage">The main page for selecting a scenario.</param>
        /// <param name="restrictedListPage">The restricted list scenario page.</param>
        /// <param name="ruleParameterPage">The rule parameter scenario page.</param>
        /// <param name="singleAccountPage">The single issue scenarios page.</param>
        /// <param name="tradingPage">The trading scenario page.</param>
        public MainPage(
            AboutPage aboutPage,
            BulkAccountPage bulkOperationPage,
            HttpClient<OpenBookHost> httpClient,
            IborPage iborPage,
            IndustryConcentrationPage industryConcentrationPage,
            ScenarioPage scenarioPage,
            MainViewModel mainViewModel,
            MasterPage masterPage,
            PackageInfo packageInfo,
            PageNavigation pageNavigation,
            IRepository repository,
            RestrictedListPage restrictedListPage,
            RuleParameterPage ruleParameterPage,
            SingleAccountPage singleAccountPage,
            TradingPage tradingPage,
            User user)
        {
            // Validate the arguments
            if (mainViewModel == null)
            {
                throw new ArgumentNullException(nameof(mainViewModel));
            }

            // Initialize the object.
            this.httpClient = httpClient;
            this.packageInfo = packageInfo;
            this.pageNavigation = pageNavigation ?? throw new ArgumentNullException(nameof(pageNavigation));
            this.repository = repository;
            this.user = user;
            this.Master = masterPage;

            // Initialize the IDE managed components.
            this.InitializeComponent();

            // The details pop-over the master only in UWP.  For the other OSs, this is the default.
            if (Device.RuntimePlatform == Device.UWP)
            {
                this.MasterBehavior = MasterBehavior.Popover;
            }

            // Provide a view model for the page.
            this.BindingContext = this.mainViewModel = mainViewModel;

            // Set the root page for the application.
            this.Detail = new NavigationPage(scenarioPage);

            // Initialize page navigator to select the proper page based on the view model.
            this.pageNavigation.Navigation = this.Detail.Navigation;
            this.pageNavigation.PageMap.Add(typeof(AboutViewModel), aboutPage);
            this.pageNavigation.PageMap.Add(typeof(BulkAccountViewModel), bulkOperationPage);
            this.pageNavigation.PageMap.Add(typeof(IborViewModel), iborPage);
            this.pageNavigation.PageMap.Add(typeof(IndustryConcentrationViewModel), industryConcentrationPage);
            this.pageNavigation.PageMap.Add(typeof(RootViewModel), scenarioPage);
            this.pageNavigation.PageMap.Add(typeof(RestrictedListViewModel), restrictedListPage);
            this.pageNavigation.PageMap.Add(typeof(RuleParameterViewModel), ruleParameterPage);
            this.pageNavigation.PageMap.Add(typeof(SingleAccountViewModel), singleAccountPage);
            this.pageNavigation.PageMap.Add(typeof(TradingViewModel), tradingPage);

            // In a background task, initialize the navigation and connection with the service.  Authenticate and then get the user's identity.
            Task task = Task.Run(async () =>
            {
                // First step, authenticate the user.
                Application.Current.Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    // Prompt the user to sign in.
                    await this.httpClient.SignInAsync().ConfigureAwait(true);
                });

                // Wait here for the authentication to complete.
                this.httpClient.Authenticated.WaitOne();

                // Now get some basic information about the user.
                User currentUser = null;
                while (currentUser == null)
                {
                    currentUser = await this.repository.GetCurrentUserAsync().ConfigureAwait(true);
                    if (currentUser == null)
                    {
                        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                        Application.Current.Dispatcher.BeginInvokeOnMainThread(async () =>
                        {
                            // Let them know that the service isn't running, give them the option to exit.
                            if (!await this.DisplayAlert(
                                this.packageInfo.Name,
                                this.mainViewModel.ServiceNotRunning,
                                this.mainViewModel.Retry,
                                this.mainViewModel.Exit)
                                .ConfigureAwait(true))
                            {
                                // The user doesn't want to wait around for the problem to resolve itself.
                                Environment.Exit(1);
                            }

                            // Tell the background task that we're done in the foreground.
                            manualResetEvent.Set();
                        });

                        // Wait for the foreground task to finish.
                        manualResetEvent.WaitOne();
                    }
                    else
                    {
                        // Note that the User member is the same shared instance of the User that we got from the Dependency Injector.  We can't
                        // replace the DI version, but we can copy the new values into it so other classes can access the current user info.
                        this.user.Name = currentUser.Name;
                        this.user.UserId = currentUser.UserId;

                        // We can now proceed with the normal operation of the app.
                        break;
                    }
                }
            });
        }
    }
}