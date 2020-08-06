/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

namespace ShipEngineAPI
{
    public partial interface IShipEngineClient
    {
        /// <summary>
        /// Sets the API key used to communicate with ShipEngine
        /// </summary>
        public string APIKey { get; set; }
    }
}
