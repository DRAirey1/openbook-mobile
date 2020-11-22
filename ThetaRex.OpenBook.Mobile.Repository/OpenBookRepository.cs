// <copyright file="OpenBookRepository.cs" company="Theta Rex, Inc.">
//    Copyright © 2020 - Theta Rex, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace ThetaRex.OpenBook.Mobile.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using ThetaRex.Common;
    using ThetaRex.OpenBook.Mobile.Common;

    /// <summary>
    /// The repository of data.
    /// </summary>
    public class OpenBookRepository : IRepository
    {
        /// <summary>
        /// The HTTP client use to communicate with the service.
        /// </summary>
        private readonly HttpClient<OpenBookHost> httpClient;

        /// <summary>
        /// The identity of the current user.
        /// </summary>
        private readonly User userIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenBookRepository"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the service.</param>
        /// <param name="userIdentity">The identity of the current user.</param>
        public OpenBookRepository(HttpClient<OpenBookHost> httpClient, User userIdentity)
        {
            // Validate the argument.
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            // Initialize the object.
            this.httpClient = httpClient;
            this.userIdentity = userIdentity;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Allocation>> AddAllocationsAsync(IEnumerable<AllocationRequest> allocationRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (allocationRequests == null)
            {
                throw new ArgumentNullException(nameof(allocationRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // We're going to use add a mass of allocations in parallel using the REST API.
            var requestUri = $"rest/allocations";
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    var realizedAllocations = new List<Allocation>();
                    var tasks = new List<Task>();
                    foreach (AllocationRequest allocationRequest in allocationRequests)
                    {
                        // Timestamp the proposed order.
                        allocationRequest.CreatedUserId = allocationRequest.ModifiedUserId = this.userIdentity.UserId;
                        allocationRequest.CreatedTime = allocationRequest.ModifiedTime = DateTime.Now;

                        // Send one or more of the allocations to thes service.  This is done in parallel, so we'll wait for all of them to finish
                        // before existing from this method.
                        tasks.Add(
                            Task.Run(
                                async () =>
                                {
                                    using (var httpContent = CreateHttpContent(allocationRequest))
                                    using (var response = await this.httpClient.PostAsync(requestUri, httpContent).ConfigureAwait(true))
                                    {
                                        // Make sure we were successful and, if so, parse the JSON data into a structure.
                                        response.EnsureSuccessStatusCode();
                                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                        realizedAllocations.Add(JsonConvert.DeserializeObject<Allocation>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                        semaphoreSlim.Release();
                                    }
                                },
                                cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);

                    // This indicates we successfully created the allocations.
                    return realizedAllocations;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProposedOrder>> AddProposedOrdersAsync(IEnumerable<ProposedOrderRequest> proposedOrderRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (proposedOrderRequests == null)
            {
                throw new ArgumentNullException(nameof(proposedOrderRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // We're going to use add a mass of proposedOrders in parallel using the REST API.
            var requestUri = $"rest/proposedOrders";
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    var realizedProposedOrders = new List<ProposedOrder>();
                    var tasks = new List<Task>();
                    foreach (ProposedOrderRequest proposedOrderRequest in proposedOrderRequests)
                    {
                        // Timestamp the proposed order.
                        proposedOrderRequest.CreatedUserId = proposedOrderRequest.ModifiedUserId = this.userIdentity.UserId;
                        proposedOrderRequest.CreatedTime = proposedOrderRequest.ModifiedTime = DateTime.Now;

                        // Send one or more of the proposed to thes service.  This is done in parallel, so we'll wait for all of them to finish
                        // before existing from this method.
                        tasks.Add(
                            Task.Run(
                                async () =>
                                {
                                    using (var httpContent = CreateHttpContent(proposedOrderRequest))
                                    using (var response = await this.httpClient.PostAsync(requestUri, httpContent).ConfigureAwait(true))
                                    {
                                        // Make sure we were successful and, if so, parse the JSON data into a structure.
                                        response.EnsureSuccessStatusCode();
                                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                        realizedProposedOrders.Add(JsonConvert.DeserializeObject<ProposedOrder>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                        semaphoreSlim.Release();
                                    }
                                },
                                cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);

                    // This indicates we successfully created the proposed orders.
                    return realizedProposedOrders;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SecurityListMap>> AddSecurityListMapsAsync(IEnumerable<SecurityListMapRequest> securityListMapRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (securityListMapRequests == null)
            {
                throw new ArgumentNullException(nameof(securityListMapRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // We're going to use add a mass of securityListMaps in parallel using the REST API.
            var requestUri = $"rest/securityListMaps";
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    var realizedSecurityListMaps = new List<SecurityListMap>();
                    var tasks = new List<Task>();
                    foreach (SecurityListMapRequest securityListMapRequest in securityListMapRequests)
                    {
                        tasks.Add(Task.Run(
                            async () =>
                            {
                                using (var httpContent = CreateHttpContent(securityListMapRequest))
                                using (var response = await this.httpClient.PostAsync(requestUri, httpContent, cancellationToken).ConfigureAwait(true))
                                {
                                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                                    response.EnsureSuccessStatusCode();
                                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                    realizedSecurityListMaps.Add(JsonConvert.DeserializeObject<SecurityListMap>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                    semaphoreSlim.Release();
                                }
                            },
                            cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);

                    // This indicates we successfully created the security list maps.
                    return realizedSecurityListMaps;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SourceOrder>> AddSourceOrdersAsync(IEnumerable<SourceOrderRequest> sourceOrderRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (sourceOrderRequests == null)
            {
                throw new ArgumentNullException(nameof(sourceOrderRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // We're going to use add a mass of sourceOrders in parallel using the REST API.
            var requestUri = "rest/sourceOrders";
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    var realizedSourceOrders = new List<SourceOrder>();
                    var tasks = new List<Task>();
                    foreach (SourceOrderRequest sourceOrderRequest in sourceOrderRequests)
                    {
                        // Timestamp the source order.
                        sourceOrderRequest.CreatedUserId = sourceOrderRequest.ModifiedUserId = this.userIdentity.UserId;
                        sourceOrderRequest.CreatedTime = sourceOrderRequest.ModifiedTime = DateTime.Now;

                        // Send one or more of the source orders to thes service.  This is done in parallel, so we'll wait for all of them to finish
                        // before existing from this method.
                        tasks.Add(Task.Run(
                            async () =>
                            {
                                using (var httpContent = CreateHttpContent(sourceOrderRequest))
                                using (var response = await this.httpClient.PostAsync(requestUri, httpContent).ConfigureAwait(true))
                                {
                                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                                    response.EnsureSuccessStatusCode();
                                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                    realizedSourceOrders.Add(JsonConvert.DeserializeObject<SourceOrder>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                    semaphoreSlim.Release();
                                }
                            },
                            cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);

                    // This indicates we successfully created the source orders.
                    return realizedSourceOrders;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TaxLot>> AddTaxLotsAsync(IEnumerable<TaxLotRequest> taxLotRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (taxLotRequests == null)
            {
                throw new ArgumentNullException(nameof(taxLotRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = "rest/taxLots";
            List<TaxLot> realizedTaxLots = new List<TaxLot>();
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    List<Task> tasks = new List<Task>();
                    foreach (TaxLotRequest taxLotRequest in taxLotRequests)
                    {
                        tasks.Add(Task.Run(
                            async () =>
                            {
                                using (var httpContent = CreateHttpContent(taxLotRequest))
                                using (var response = await this.httpClient.PostAsync(requestUri, httpContent).ConfigureAwait(true))
                                {
                                    // Make sure we were successful and, if so, put the resulting records in a list making sure we don't step on any
                                    // other tasks trying to access the list.
                                    response.EnsureSuccessStatusCode();
                                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                    realizedTaxLots.Add(JsonConvert.DeserializeObject<TaxLot>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                    semaphoreSlim.Release();
                                }
                            },
                            cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This is the realized list of tax lots.
            return realizedTaxLots;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkingOrder>> AddWorkingOrdersAsync(IEnumerable<WorkingOrderRequest> workingOrderRequests, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (workingOrderRequests == null)
            {
                throw new ArgumentNullException(nameof(workingOrderRequests));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // We're going to use add a mass of workingOrders in parallel using the REST API.
            var requestUri = "rest/workingOrders";
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to add every tax lot in the list.
                    var realizedWorkingOrders = new List<WorkingOrder>();
                    var tasks = new List<Task>();
                    foreach (WorkingOrderRequest workingOrderRequest in workingOrderRequests)
                    {
                        // Timestamp the working order.
                        workingOrderRequest.CreatedUserId = workingOrderRequest.ModifiedUserId = this.userIdentity.UserId;
                        workingOrderRequest.CreatedTime = workingOrderRequest.ModifiedTime = DateTime.Now;

                        // Send one or more of the working orders to thes service.  This is done in parallel, so we'll wait for all of them to finish
                        // before existing from this method.
                        tasks.Add(Task.Run(
                            async () =>
                            {
                                using (var httpContent = CreateHttpContent(workingOrderRequest))
                                using (var response = await this.httpClient.PostAsync(requestUri, httpContent).ConfigureAwait(true))
                                {
                                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                                    response.EnsureSuccessStatusCode();
                                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                    realizedWorkingOrders.Add(JsonConvert.DeserializeObject<WorkingOrder>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                    semaphoreSlim.Release();
                                }
                            },
                            cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);

                    // This indicates we successfully created the working orders.
                    return realizedWorkingOrders;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkingOrder>> BulkAddWorkingOrdersAsync(IEnumerable<WorkingOrder> workingOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (workingOrders == null)
            {
                throw new ArgumentNullException(nameof(workingOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Unlike the other REST APIs that execute single calls in parallel, this one is a batch API.
            var requestUri = "transaction/workingOrders";
            try
            {
                // Start running a parallel task to add every tax lot in the list.
                var realizedWorkingOrders = new List<WorkingOrder>();
                foreach (WorkingOrder workingOrder in workingOrders)
                {
                    // Timestamp the working order and the associated source orders.
                    workingOrder.CreatedUserId = workingOrder.ModifiedUserId = this.userIdentity.UserId;
                    workingOrder.CreatedTime = workingOrder.ModifiedTime = DateTime.Now;
                    foreach (SourceOrder sourceOrder in workingOrder.SourceOrders)
                    {
                        sourceOrder.CreatedUserId = sourceOrder.ModifiedUserId = this.userIdentity.UserId;
                        sourceOrder.CreatedTime = sourceOrder.ModifiedTime = DateTime.Now;
                    }
                }

                // Post the working orders to the web service.
                using (var httpContent = CreateHttpContent(workingOrders))
                using (var response = await this.httpClient.PostAsync(requestUri, httpContent, cancellationToken).ConfigureAwait(true))
                {
                    response.EnsureSuccessStatusCode();
                    workingOrders = JsonConvert.DeserializeObject<IEnumerable<WorkingOrder>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));

                    // This indicates we successfully created the working orders.
                    return workingOrders;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This indicates a failure.
            return null;
        }

        /// <inheritdoc/>
        public async Task<bool> BulkDeleteWorkingOrdersAsync(IEnumerable<WorkingOrder> workingOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (workingOrders == null)
            {
                throw new ArgumentNullException(nameof(workingOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every working order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (WorkingOrder workingOrder in workingOrders)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the working order.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"transaction/workingOrders/{workingOrder.WorkingOrderId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{workingOrder.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful.  404 - 'Not Found' - on a delete is not considered an error.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAlertsAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (alerts == null)
            {
                throw new ArgumentNullException(nameof(alerts));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every tax lot that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (Alert alert in alerts)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the tax lot.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/alerts/{alert.AlertId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{alert.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllocationsAsync(IEnumerable<Allocation> allocations, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (allocations == null)
            {
                throw new ArgumentNullException(nameof(allocations));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every allocation that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (Allocation allocation in allocations)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the allocation.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/allocations/{allocation.AllocationId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{allocation.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data
                                    // into a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProposedOrdersAsync(IEnumerable<ProposedOrder> proposedOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (proposedOrders == null)
            {
                throw new ArgumentNullException(nameof(proposedOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every proposed order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (ProposedOrder proposedOrder in proposedOrders)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the proposed order.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/proposedOrders/{proposedOrder.ProposedOrderId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{proposedOrder.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSecurityListMapsAsync(IEnumerable<SecurityListMap> securityListMaps, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (securityListMaps == null)
            {
                throw new ArgumentNullException(nameof(securityListMaps));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Ask for the JSON data at the requested URI.
            try
            {
                foreach (SecurityListMap securityListMap in securityListMaps)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/securityListMaps/{securityListMap.SecurityListMapId}"))
                    {
                        request.Headers.Add("If-None-Match", $"\"{securityListMap.RowVersion}\"");
                        using (HttpResponseMessage response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(true))
                        {
                            // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                            // a structure.
                            if (response.StatusCode != HttpStatusCode.NotFound)
                            {
                                response.EnsureSuccessStatusCode();
                            }
                        }
                    }
                }

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteDestinationOrdersAsync(IEnumerable<DestinationOrder> destinationOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (destinationOrders == null)
            {
                throw new ArgumentNullException(nameof(destinationOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every source order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (DestinationOrder destinationOrder in destinationOrders)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the source order.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/destinationOrders/{destinationOrder.DestinationOrderId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{destinationOrder.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data
                                    // into a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteExecutionsAsync(IEnumerable<Execution> executions, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (executions == null)
            {
                throw new ArgumentNullException(nameof(executions));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every source order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (Execution execution in executions)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the execution.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/executions/{execution.ExecutionId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{execution.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSourceOrdersAsync(IEnumerable<SourceOrder> sourceOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (sourceOrders == null)
            {
                throw new ArgumentNullException(nameof(sourceOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every source order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (SourceOrder sourceOrder in sourceOrders)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the source order.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/sourceOrders/{sourceOrder.SourceOrderId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{sourceOrder.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteTaxLotsAsync(IEnumerable<TaxLot> taxLots, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (taxLots == null)
            {
                throw new ArgumentNullException(nameof(taxLots));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every tax lot that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (TaxLot taxLot in taxLots)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the tax lot.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/taxLots/{taxLot.TaxLotId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{taxLot.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWorkingOrdersAsync(IEnumerable<WorkingOrder> workingOrders, CancellationToken cancellationToken = default)
        {
            // Validate the argument
            if (workingOrders == null)
            {
                throw new ArgumentNullException(nameof(workingOrders));
            }

            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            try
            {
                // Start running a parallel task to delete every working order that needs to be deleted.
                List<Task> tasks = new List<Task>();
                foreach (WorkingOrder workingOrder in workingOrders)
                {
                    tasks.Add(Task.Run(
                        async () =>
                        {
                            // Use the REST API to delete the working order.
                            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"rest/workingOrders/{workingOrder.WorkingOrderId}"))
                            {
                                // This header allows the web service to check for optimistic concurrency violations.
                                request.Headers.Add("If-None-Match", $"\"{workingOrder.RowVersion}\"");
                                using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                {
                                    // Make sure we were successful ('Not Found' on a delete is not considered an error) and, if so, parse the JSON data into
                                    // a structure.
                                    if (response.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        },
                        cancellationToken));
                }

                // Wait here for all the parallel tasks to finish.
                await Task.WhenAll(tasks).ConfigureAwait(true);

                // If we got here, then success.
                return true;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // Failed at some level.
            return false;
        }

        /// <inheritdoc/>
        public async Task<Account> GetAccountByAccountExternalKeyAsync(string mnemonic, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to request the account.
            var requestUri = $"rest/accounts/accountExternalKey/{mnemonic}";
            Account account = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    account = JsonConvert.DeserializeObject<Account>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The requested account.
            return account;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Account>> GetAccountsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to request the account.
            var requestUri = "rest/accounts";
            IEnumerable<Account> accounts = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    accounts = JsonConvert.DeserializeObject<IEnumerable<Account>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of accounts.
            return accounts;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/alerts";
            List<Alert> alerts = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    alerts = JsonConvert.DeserializeObject<List<Alert>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            return alerts;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Allocation>> GetAllocationsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/allocations";
            List<Allocation> allocations = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    allocations = JsonConvert.DeserializeObject<List<Allocation>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of proposed orders.
            return allocations;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Blotter>> GetBlottersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to request the account.
            var requestUri = "rest/blotters";
            IEnumerable<Blotter> blotters = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    blotters = JsonConvert.DeserializeObject<IEnumerable<Blotter>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of accounts.
            return blotters;
        }

        /// <inheritdoc/>
        public async Task<CategoryBenchmark> GetCategoryBenchmarkByCategoryBenchmarkExternalKeyAsync(string externalId, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/categoryBenchmarks/categoryBenchmarkExternalKey/{externalId}";
            CategoryBenchmark categoryBenchmark = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    categoryBenchmark = JsonConvert.DeserializeObject<CategoryBenchmark>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of proposed orders.
            return categoryBenchmark;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DestinationOrder>> GetDestinationOrdersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) source order.
            var requestUri = $"rest/destinationOrders";
            List<DestinationOrder> destinationOrders = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    destinationOrders = JsonConvert.DeserializeObject<List<DestinationOrder>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of source orders.
            return destinationOrders;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Execution>> GetExecutionsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) source order.
            var requestUri = $"rest/executions";
            List<Execution> executions = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    executions = JsonConvert.DeserializeObject<List<Execution>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of source orders.
            return executions;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ManagedAccount>> GetManagedAccountsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to request the account.
            var requestUri = "rest/managedaccounts";
            IEnumerable<ManagedAccount> accounts = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    accounts = JsonConvert.DeserializeObject<IEnumerable<ManagedAccount>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of accounts.
            return accounts;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Price>> GetPricesAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/prices";
            List<Price> prices = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    prices = JsonConvert.DeserializeObject<List<Price>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The requested security.
            return prices;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProposedOrder>> GetProposedOrdersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/proposedOrders";
            List<ProposedOrder> proposedOrders = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    proposedOrders = JsonConvert.DeserializeObject<List<ProposedOrder>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of proposed orders.
            return proposedOrders;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Security>> GetSecuritiesAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/securities";
            List<Security> securities = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    securities = JsonConvert.DeserializeObject<List<Security>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The requested security.
            return securities;
        }

        /// <inheritdoc/>
        public async Task<Security> GetSecurityByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/securities/securityExternalKey/{externalId}";
            Security security = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    security = JsonConvert.DeserializeObject<Security>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The requested security.
            return security;
        }

        /// <inheritdoc/>
        public async Task<Security> GetSecurityByFigiAsync(string figi, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/securities/securityFigiKey/{figi}";
            Security security = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    security = JsonConvert.DeserializeObject<Security>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The requested security.
            return security;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SecurityListMap>> GetSecurityListMapsAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) proposed order.
            var requestUri = $"rest/securityListMaps";
            List<SecurityListMap> securityListMaps = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    securityListMaps = JsonConvert.DeserializeObject<List<SecurityListMap>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of proposed orders.
            return securityListMaps;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SourceOrder>> GetSourceOrdersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) source order.
            var requestUri = $"rest/sourceOrders";
            List<SourceOrder> sourceOrders = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    sourceOrders = JsonConvert.DeserializeObject<List<SourceOrder>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of source orders.
            return sourceOrders;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TaxLot>> GetTaxLotsAsync(int accountId, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) source order.
            var requestUri = $"transaction/taxLots/accountKey/{accountId}";
            List<TaxLot> taxLots = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    taxLots = JsonConvert.DeserializeObject<List<TaxLot>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of tax lots.
            return taxLots;
        }

        /// <inheritdoc/>
        public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) working order.
            var requestUri = $"transaction/currentUser";
            User user = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The user information.
            return user;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkingOrder>> GetWorkingOrdersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Call the API to add a batch of stamped (time and user stamped) working order.
            var requestUri = $"rest/workingOrders";
            List<WorkingOrder> workingOrders = null;
            try
            {
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(true))
                {
                    // Make sure we were successful and, if so, parse the JSON data into a structure.
                    response.EnsureSuccessStatusCode();
                    workingOrders = JsonConvert.DeserializeObject<List<WorkingOrder>>(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // The list of working orders.
            return workingOrders;
        }

        /// <inheritdoc/>
        public async Task SendOrdersAsync(CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Post the command that will create the destination orders which will then be sent to the broker for execution.
            var requestUri = $"transaction/orders/SendOrders";
            try
            {
                using (var response = await this.httpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(true))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <inheritdoc/>
        public async Task<List<CategoryBenchmark>> UpdateClassificationBenchmarkAsync(IEnumerable<CategoryBenchmark> categoryBenchmarks, CancellationToken cancellationToken = default)
        {
            // Wait for an authenticated client.
            this.httpClient.Authenticated.WaitOne();

            // Validate the argument
            if (categoryBenchmarks == null)
            {
                throw new ArgumentNullException(nameof(categoryBenchmarks));
            }

            // Call the API to add a batch of classification benchmark records.
            List<CategoryBenchmark> realizedCategoryBenchmarks = new List<CategoryBenchmark>();
            try
            {
                // The semaphore allows multiple parallel tasks to update the list containing the results.
                using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
                {
                    // Start running a parallel task to delete every tax lot that needs to be deleted.
                    List<Task> tasks = new List<Task>();
                    foreach (CategoryBenchmark categoryBenchmark in categoryBenchmarks)
                    {
                        tasks.Add(Task.Run(
                            async () =>
                            {
                                // Use the REST API to update the record.
                                using (var request = new HttpRequestMessage(HttpMethod.Put, $"rest/categoryBenchmarks/categoryBenchmarkKey/{categoryBenchmark.CategoryBenchmarkId}"))
                                {
                                    // This header allows the web service to check for optimistic concurrency violations.
                                    request.Headers.Add("If-None-Match", $"\"{categoryBenchmark.RowVersion}\"");
                                    using (request.Content = CreateHttpContent(categoryBenchmark))
                                    using (HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(true))
                                    {
                                        response.EnsureSuccessStatusCode();
                                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                                        realizedCategoryBenchmarks.Add(JsonConvert.DeserializeObject<CategoryBenchmark>(await response.Content.ReadAsStringAsync().ConfigureAwait(true)));
                                        semaphoreSlim.Release();
                                    }
                                }
                            },
                            cancellationToken));
                    }

                    // Wait here for all the parallel tasks to finish.
                    await Task.WhenAll(tasks).ConfigureAwait(true);
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            // This is the realized list updated classification benchmarks.
            return realizedCategoryBenchmarks;
        }

        /// <summary>
        /// Creates JSON content.
        /// </summary>
        /// <param name="content">The original content.</param>
        /// <returns>The serialized content in a JSON format.</returns>
        private static HttpContent CreateHttpContent(object content)
        {
            // Creat the JSON content.
            HttpContent httpContent = null;
            if (content != null)
            {
                httpContent = new StringContent(JsonConvert.SerializeObject(content));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            // This is the JSON message payload.
            return httpContent;
        }
    }
}