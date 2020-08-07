/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShipEngineAPI;
// ReSharper disable StringLiteralTypo

namespace ShipEngineTest
{
    public static class Program
    {
        public static async Task Main()
        {
            const string toParse = "424 Otterson Drive, Chico CA 95928";
            Console.WriteLine($"Parsing: {toParse}");

            try
            {
                using var httpClient = new SingleInstanceHttpClient();
                var client = new ShipEngineClient(httpClient)
                {
                    APIKey = "TEST_UviO3xlrk2l3tdqyTLUZ9flQuDtVwRBTUlVZgZmCQA4",
                };
                var response = await client.ParseAddress(new ParseAddressRequestBody
                {
                    Text = toParse,
                });

                Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED!");
                Console.WriteLine(e.Message);
            }
        }
    }
}