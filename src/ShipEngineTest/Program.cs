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
                var client = new ShipEngineClient(httpClient, apiKey) as IShipEngineClient;
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

                // Check an international address which gives some errors
                validated = await client.ValidateAddress(new[] {
                    new AddressToValidate {
                        AddressLine1 = "Villa Granada 122",
                        CityLocality = "Saltillo",
                        StateProvince = "Coahuila",
                        PostalCode = "25210",
                        CountryCode = "MX",
                    },
                });
                Console.WriteLine(JsonConvert.SerializeObject(validated, Formatting.Indented));

                // Get a list of available carriers
                var carriers = await client.ListCarriers();
                Console.WriteLine(JsonConvert.SerializeObject(carriers, Formatting.Indented));

                // Find some domestic entries to print labels for
                var rateResponse = await GenerateDomesticRate(client, carriers);
                Console.WriteLine(JsonConvert.SerializeObject(rateResponse, Formatting.Indented));
                var domesticUSPSRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "usps_priority_mail");
                Console.WriteLine(JsonConvert.SerializeObject(domesticUSPSRate, Formatting.Indented));

                // Now get UPS ground
                rateResponse = await GenerateDomesticRate(client, carriers);
                var domesticUPSRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "ups_ground");
                Console.WriteLine(JsonConvert.SerializeObject(domesticUPSRate, Formatting.Indented));

                // Now get FedEx ground
                rateResponse = await GenerateDomesticRate(client, carriers);
                var domesticFedExRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "fedex_ground");
                Console.WriteLine(JsonConvert.SerializeObject(domesticFedExRate, Formatting.Indented));

                // Get rates for priority mail international entry for later
                rateResponse = await GenerateInternationalRate(client, carriers);
                Console.WriteLine(JsonConvert.SerializeObject(rateResponse, Formatting.Indented));
                var internationalUSPSRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "usps_priority_mail_international");
                Console.WriteLine(JsonConvert.SerializeObject(internationalUSPSRate, Formatting.Indented));

                // Now get UPS
                rateResponse = await GenerateInternationalRate(client, carriers);
                var internationalUPSRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "ups_worldwide_expedited");
                Console.WriteLine(JsonConvert.SerializeObject(internationalUPSRate, Formatting.Indented));

                // Now get FedEx
                rateResponse = await GenerateInternationalRate(client, carriers);
                var internationalFedExRate = rateResponse.RateResponse.Rates.FirstOrDefault(p => p.ServiceCode == "fedex_international_economy");
                Console.WriteLine(JsonConvert.SerializeObject(internationalFedExRate, Formatting.Indented));

                // Now create labels for the two USPS shipments
                var labelRequest = new CreateLabelFromRateRequestBody {
                    LabelLayout = LabelLayout._4x6,
                    LabelFormat = LabelFormat.Pdf,
                    LabelDownloadType = LabelDownloadType.Url,
                    //TestLabel = true,
                };
                var labelResponse = await client.CreateLabelFromRate(labelRequest, domesticUSPSRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var domesticUSPSLabelId = labelResponse.LabelId;
                labelResponse = await client.CreateLabelFromRate(labelRequest, internationalUSPSRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var internationalUSPSLabelId = labelResponse.LabelId;

                // Now create labels for the two UPS shipments
                labelResponse = await client.CreateLabelFromRate(labelRequest, domesticUPSRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var domesticUPSLabelId = labelResponse.LabelId;
                labelResponse = await client.CreateLabelFromRate(labelRequest, internationalUPSRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var internationalUPSLabelId = labelResponse.LabelId;

                // Now create labels for the two FedEx shipments
                labelResponse = await client.CreateLabelFromRate(labelRequest, domesticFedExRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var domesticFedExLabelId = labelResponse.LabelId;
                labelResponse = await client.CreateLabelFromRate(labelRequest, internationalFedExRate.RateId);
                Console.WriteLine(JsonConvert.SerializeObject(labelResponse, Formatting.Indented));
                var internationalFedExLabelId = labelResponse.LabelId;

                // Try to create a manifest for the USPS labels
                var manifestRequest = new CreateManifestRequestBody {
                    LabelIds = new List<string> {
                        domesticUSPSLabelId,
                        internationalUSPSLabelId,
                    },
                };
                var result = await client.CreateManifest(manifestRequest);
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                // Try to create a manifest for the UPS labels. This should fail with a not implemented exception
                try {
                    manifestRequest = new CreateManifestRequestBody {
                        LabelIds = new List<string> {
                            domesticUPSLabelId,
                            internationalUPSLabelId,
                        },
                    };
                    result = await client.CreateManifest(manifestRequest);
                    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                } catch (ApiException<ErrorResponseBody> e) {
                    var error = e.Result.Errors[0];
                    if (error.ErrorType != ErrorType.BusinessRules || error.Message != "The method or operation is not implemented.") {
                        Console.WriteLine(e.Message);
                    }
                }

                // Try to create a manifest for the FedEx labels
                manifestRequest = new CreateManifestRequestBody {
                    LabelIds = new List<string> {
                        domesticFedExLabelId,
                        internationalFedExLabelId,
                    },
                };
                result = await client.CreateManifest(manifestRequest);
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            } catch (Exception e) {
                Console.WriteLine("FAILED!");
                Console.WriteLine(e.Message);
                if (e.InnerException != null) {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
        }

        /// <summary>
        /// Generate a domestic rate
        /// </summary>
        private static async Task<CalculateRatesResponseBody> GenerateDomesticRate(
            IShipEngineClient client,
            GetCarriersResponseBody carriers)
        {
            return await client.CalculateRates(new CalculateRatesRequestBody {
                Shipment = new AddressValidatingShipment {
                    ValidateAddress = ValidateAddress.NoValidation,
                    ShipDate = DateTimeOffset.Now.Date,
                    ShipTo = new Address {
                        Name = "Joe Blogs",
                        Phone = "1-530-894-0797",
                        AddressLine1 = "411 Otterson Drive",
                        CityLocality = "Chico",
                        StateProvince = "CA",
                        PostalCode = "95928-8217",
                        CountryCode = "US",
                    },
                    ShipFrom = new Address {
                        Name = "Online Seller",
                        Phone = "1-530-894-0797",
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
        }

        /// <summary>
        /// Generate an international rate
        /// </summary>
        private static async Task<CalculateRatesResponseBody> GenerateInternationalRate(
            IShipEngineClient client,
            GetCarriersResponseBody carriers)
        {
            return await client.CalculateRates(new CalculateRatesRequestBody {
                Shipment = new AddressValidatingShipment {
                    ValidateAddress = ValidateAddress.NoValidation,
                    ShipDate = DateTimeOffset.Now.Date,
                    ShipTo = new Address {
                        Name = "Joe Blogs",
                        Phone = "1-530-894-0797",
                        AddressLine1 = "81 Packingston St",
                        CityLocality = "Kew",
                        StateProvince = "VIC",
                        PostalCode = "3101",
                        CountryCode = "AU",
                    },
                    ShipFrom = new Address {
                        Name = "Online Seller",
                        Phone = "1-530-894-0797",
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
                    Customs = new InternationalShipmentOptions {
                        Contents = PackageContents.Merchandise,
                        NonDelivery = NonDelivery.ReturnToSender,
                        CustomsItems = new List<CustomsItem> {
                            new CustomsItem {
                                Description = "Product name",
                                Quantity = 2,
                                Value = 42.99,
                                HarmonizedTariffCode = "",
                                CountryOfOrigin = "US",
                                Sku = "PRODUCT_SKU",
                            },
                        },
                    },
                },
                RateOptions = new RateRequestBody {
                    CarrierIds = carriers.Carriers.Select(p => p.CarrierId).ToList(),
                },
            });
        }
   }
}