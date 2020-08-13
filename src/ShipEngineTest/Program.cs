/*
 * Copyright (C) 2020 and later AMain.com, Inc.
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
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
            try {
                const string apiKey = "TEST_UviO3xlrk2l3tdqyTLUZ9flQuDtVwRBTUlVZgZmCQA4";
                using var httpClient = new SingleInstanceHttpClient();
                var client = new ShipEngineClient(httpClient, apiKey);
                client.ReadResponseAsString = true;
                const string toParse = "424 Otterson Drive, Chico CA 95928";
                Console.WriteLine($"Parsing: {toParse}");
                var response = await client.ParseAddress(new ParseAddressRequestBody {
                    Text = toParse,
                });
                Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));

                // Now try to validate the address
                var validated = await client.ValidateAddress(new[] {
                    new AddressToValidate {
                        AddressLine1 = "100 Unknown Street",
                        CityLocality = "Chico",
                        StateProvince = "CA",
                        PostalCode = "95973",
                        CountryCode = "US",
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(validated, Formatting.Indented));
                validated = await client.ValidateAddress(new[] {
                    new AddressToValidate {
                        AddressLine1 = "424 Otterson Drive",
                        CityLocality = "Chico",
                        StateProvince = "CA",
                        PostalCode = "95928",
                        CountryCode = "US",
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(validated, Formatting.Indented));

                // Get a list of available carriers
                var carriers = await client.ListCarriers();
                Console.WriteLine(JsonConvert.SerializeObject(carriers, Formatting.Indented));

                // Try to rate a package
                var rates = await client.CalculateRates(new CalculateRatesRequestBody {
                    Shipment = new AddressValidatingShipment {
                        ValidateAddress = ValidateAddress.NoValidation,
                        ShipDate = DateTimeOffset.Now.Date,
                        ShipTo = new Address {
                            Name = "Joe Blogs",
                            Phone = "N/A",
                            AddressLine1 = "411 Otterson Drive",
                            CityLocality = "Chico",
                            StateProvince = "CA",
                            PostalCode = "95928-8217",
                            CountryCode = "US",
                        },
                        ShipFrom = new Address {
                            Name = "Online Seller",
                            Phone = "N/A",
                            AddressLine1 = "424 Otterson Drive",
                            CityLocality = "Chico",
                            StateProvince = "CA",
                            PostalCode = "95928-8217",
                            CountryCode = "US",
                        },
                        Packages = new List<Package> {
                            new Package {
                                Weight = new Weight {
                                    Value = 1.0,
                                    Unit = WeightUnit.Pound,
                                },
                                Dimensions = new Dimensions {
                                    Length = 1,
                                    Width = 2,
                                    Height = 3,
                                },
                            },
                        },
                    },
                    RateOptions = new RateRequestBody {
                        CarrierIds = carriers.Carriers.Select(p => p.CarrierId).ToList(),
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(rates, Formatting.Indented));
            } catch (Exception e) {
                Console.WriteLine("FAILED!");
                Console.WriteLine(e.Message);
                if (e.InnerException != null) {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
        }
    }
}