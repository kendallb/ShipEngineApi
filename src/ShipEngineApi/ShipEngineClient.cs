/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System.Net.Http;
// ReSharper disable UnusedParameterInPartialMethod

namespace ShipEngineAPI
{
    public partial class ShipEngineClient
    {
        /// <summary>
        /// Sets the API key used to communicate with ShipEngine
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Internal function to prepare the request
        /// </summary>
        /// <param name="client">HttpClient being used</param>
        /// <param name="request">HttpRequestMessage for this request</param>
        /// <param name="url">Url for the request</param>
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            request.Headers.Add("API-Key", APIKey);
        }
    }
}
