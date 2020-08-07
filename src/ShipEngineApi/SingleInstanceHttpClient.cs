﻿/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System.Net.Http;

namespace ShipEngineAPI
{
    /// <summary>
    /// Class for a single instance of HttpClient. You should only ever instantiate a single version of this class
    /// in your code, and reuse it across all requests:
    ///
    /// https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
    ///
    /// </summary>
    public class SingleInstanceHttpClient : HttpClient, IHttpClient
    {
    }
}
