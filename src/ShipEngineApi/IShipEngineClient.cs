/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

namespace ShipEngineAPI
{
    public partial interface IShipEngineClient
    {
        /// <summary>
        /// True to read the response as a string rather than as a stream, which provides for better error reporting
        /// </summary>
        bool ReadResponseAsString { get; set; }
    }
}