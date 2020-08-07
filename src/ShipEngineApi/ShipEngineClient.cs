/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System;
using System.Net.Http;
// ReSharper disable UnusedParameterInPartialMethod

namespace ShipEngineAPI
{
    public partial class ShipEngineClient
    {
        private readonly string _apiKey;

        /// <summary>
        /// Constructor for the ShipEngineClient class
        /// </summary>
        /// <param name="httpClient">Reference to the shared HttpClient class to use</param>
        /// <param name="apiKey">Reference to the API keys to use for the API calls</param>
        public ShipEngineClient(IHttpClient httpClient, string apiKey)
            : this(httpClient)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Internal function to prepare the request
        /// </summary>
        /// <param name="client">HttpClient being used</param>
        /// <param name="request">HttpRequestMessage for this request</param>
        /// <param name="url">Url for the request</param>
        partial void PrepareRequest(IHttpClient client, HttpRequestMessage request, string url)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentException("API key was not provided. Please include the API key when you create the ShipEngineClient instance!");
            }
            request.Headers.Add("API-Key", _apiKey);
        }
    }
}
