// <copyright file="MainPage.xaml.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace ThetaRex.OpenBook.Mobile.Views
{
    using System;
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
        /// <param name="aboutPage">The 'About' page.</param>
        /// <param name="bulkOperationPage">The bulk opreations scenarios page.</param>
        /// <param name="changePricePage">The price change scenario page.</param>
        /// <param name="httpClient">The HTTP client used to communicate with the service.</param>
        /// <param name="iborPage">The IBOR scenario page.</param>
        /// <param name="industryConcentrationPage">The industry concentration scenario page.</param>
        /// <param name="mainViewModel">The view model for the main page.</param>
        /// <param name="masterPage">The view for the master in the master-detail view.</param>
        /// <param name="navigator">The translation between view model navigation and view navigation.</param>
        /// <param name="packageInfo">Information about the package.</param>
        /// <param name="repository">The repository where data is read and written.</param>
        /// <param name="restrictedListPage">The restricted list scenario page.</param>
        /// <param name="ruleParameterPage">The rule parameter scenario page.</param>
        /// <param name="scenarioPage">The main page for selecting a scenario.</param>
        /// <param name="singleAccountPage">The single issue scenarios page.</param>
        /// <param name="tradingPage">The trading scenario page.</param>
        /// <param name="user">The identity of the current user.</param>
        public MainPage(
            AboutPage aboutPage,
            BulkAccountPage bulkOperationPage,
            ChangePricePage changePricePage,
            HttpClient<OpenBookHost> httpClient,
            IborPage iborPage,
            IndustryConcentrationPage industryConcentrationPage,
            MainViewModel mainViewModel,
            MasterPage masterPage,
            Navigator navigator,
            PackageInfo packageInfo,
            IRepository repository,
            RestrictedListPage restrictedListPage,
            RuleParameterPage ruleParameterPage,
            ScenarioPage scenarioPage,
            SingleAccountPage singleAccountPage,
            TradingPage tradingPage,
            User user)
        {
            // Validate the argument.
            if (mainViewModel == null)
            {
                throw new ArgumentNullException(nameof(mainViewModel));
            }

            // Validate the argument.
            if (navigator == null)
            {
                throw new ArgumentNullException(nameof(navigator));
            }

            // Initialize the object.
            this.httpClient = httpClient;
            this.packageInfo = packageInfo;
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

            // Initialize the navigator to select the proper view based on the view model.
            navigator.Navigation = this.Detail.Navigation;
            navigator.PageMap.Add(typeof(AboutViewModel), aboutPage);
            navigator.PageMap.Add(typeof(BulkAccountViewModel), bulkOperationPage);
            navigator.PageMap.Add(typeof(IborViewModel), iborPage);
            navigator.PageMap.Add(typeof(IndustryConcentrationViewModel), industryConcentrationPage);
            navigator.PageMap.Add(typeof(RootViewModel), scenarioPage);
            navigator.PageMap.Add(typeof(RestrictedListViewModel), restrictedListPage);
            navigator.PageMap.Add(typeof(RuleParameterViewModel), ruleParameterPage);
            navigator.PageMap.Add(typeof(SingleAccountViewModel), singleAccountPage);
            navigator.PageMap.Add(typeof(TradingViewModel), tradingPage);
            navigator.PageMap.Add(typeof(ChangePriceViewModel), changePricePage);

            // Authenticate the user and initialize the data domain in a background task.
            Task task = Task.Run(this.InitializeDataDomainAsync);
        }

        /// <summary>
        /// Initialize the data domain.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private async Task InitializeDataDomainAsync()
        {
            // Get some basic information about the user.
            User currentUser = null;
            while (currentUser == null)
            {
                currentUser = await this.repository.GetCurrentUserAsync().ConfigureAwait(true);
                if (currentUser == null)
                {
                    SemaphoreSlim waitForMessage = new SemaphoreSlim(1);
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
                        waitForMessage.Release();
                    });

                    // Wait for the foreground task to finish.
                    await waitForMessage.WaitAsync().ConfigureAwait(false);
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
        }
    }
}