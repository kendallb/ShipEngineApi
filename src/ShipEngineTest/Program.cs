/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System;
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
                var client = new ShipEngineClient(httpClient, "TEST_UviO3xlrk2l3tdqyTLUZ9flQuDtVwRBTUlVZgZmCQA4");
                var response = await client.ParseAddress(new ParseAddressRequestBody
                {
                    Text = toParse,
                });
                Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));

                // Now try to validate the address
                var validated = await client.ValidateAddress(new [] {
                    new AddressToValidate
                    {
                        AddressLine1 = "424 Otterson Drive, Suite 100",
                        CityLocality = "Chico",
                        StateProvince = "CA",
                        PostalCode = "95928",
                        CountryCode = "US",
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(validated, Formatting.Indented));

                validated = await client.ValidateAddress(new [] {
                    new AddressToValidate
                    {
                        AddressLine1 = "424 Otterson Drive",
                        CityLocality = "Chico",
                        StateProvince = "CA",
                        PostalCode = "95928",
                        CountryCode = "US",
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(validated, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED!");
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
        }
    }
}