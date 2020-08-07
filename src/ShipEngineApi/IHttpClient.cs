﻿/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

 using System;
 using System.Net.Http;
 using System.Threading;
 using System.Threading.Tasks;

 namespace ShipEngineAPI
{
    /// <summary>
    /// Interface for the single instance HttpClient we use in the API calls
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
        /// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken);
    }
}
