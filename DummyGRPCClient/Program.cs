
using CarriersClient;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using DispatchClient;
using ShipmentClient;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

//Create Configuration
var _configuration = new ConfigurationBuilder()
        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")        
        .Build();

//Create Grpc Channel and carrier client
using var channel = GrpcChannel.ForAddress(_configuration.GetSection("ServerEndpoint").Value);

//connect to shipment service
var client = new ShipmentClient.Shipments.ShipmentsClient(channel);
IPublicClientApplication _publicClientApplication = CreatePublicClientApplication();
var headers = await CreateAuthMetadataAsync();
if (headers == null)
{
    Console.WriteLine("Couldn't get accessToken.");
    return;
}
var x = await client.ListAsync(new ListRequest() { }, headers);
Console.WriteLine($"shipment response {x.Result.Count} {x.Result}");

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//connect to driver service
using var channel1 = GrpcChannel.ForAddress(_configuration.GetSection("DriverEndpoint").Value);
var driver_client = new DriverClient.Drivers.DriversClient(channel1);
var x1 = await driver_client.ListAsync(new DriverClient.ListRequest());
Console.WriteLine($"driver response {x1.Result.Count} {x1.Result}");


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using var channel2 = GrpcChannel.ForAddress(_configuration.GetSection("DispatchEndpoint").Value);
var dispatch_client = new DispatchClient.Dispatch.DispatchClient(channel2);
var replyd = await dispatch_client.GetDummyAsync(new DummyRequest() { Name = "Pallavi" }, headers);
Console.WriteLine("Dispatch o/p =>" + replyd.Message);


//new CarriersClient.Carriers.CarriersClient(channel);
//Create PublicClientApplication
//create header object for grpc request
// The port number must match the port of the gRPC server.
//var reply = await client.GetCountryCarriersAsync(
//                  new GetCountryCarriersRequest() { Name = "DummyClient" }, headers);
//Console.WriteLine("CountryCarriers count: " + reply.CountryCarriers.Count + " Countries:- " + reply.CountryCarriers[0].Country + "," + reply.CountryCarriers[1].Country);




Console.WriteLine("Press any key to exit...");


Console.ReadKey();



IPublicClientApplication CreatePublicClientApplication()
{
    var options = new PublicClientApplicationOptions();
    _configuration.Bind("AzureAd", options);
    return PublicClientApplicationBuilder.CreateWithApplicationOptions(options)
        .Build();
}

async Task<Metadata> CreateAuthMetadataAsync()
{
    //var r = await AcquireTokenAsync();
    //if (r == null)
    //{
    //    return null;
    //}

    //return new Metadata
    //{
    //    { "Authorization", $"Bearer {r.AccessToken}" },
    //};

    //creating token using postman
    return new Metadata
    {
        { "Authorization", $"Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJmZTVhN2FkNy00Y2U4LTQ5MzItYWViYi0wMGFkODBlMDE0ZmIiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vNDFmZjI2ZGMtMjUwZi00YjEzLTg5ODEtNzM5YmU4NjEwYzIxL3YyLjAiLCJpYXQiOjE2NTg4MzA5MzIsIm5iZiI6MTY1ODgzMDkzMiwiZXhwIjoxNjU4OTEwNDMyLCJhaW8iOiJFMlpnWUlpTis2MytMdVhtdnduYXAxd3ZQdDkwRkFBPSIsImF6cCI6ImZlNWE3YWQ3LTRjZTgtNDkzMi1hZWJiLTAwYWQ4MGUwMTRmYiIsImF6cGFjciI6IjEiLCJvaWQiOiI3YjdhYjg5ZC01ZjA3LTRmODAtYjU0OS0zNTMwMDBiYjQ5MmQiLCJyaCI6IjAuQVJFQTNDYl9RUThsRTB1SmdYT2I2R0VNSWRkNld2N29UREpKcnJzQXJZRGdGUHNSQUFBLiIsInN1YiI6IjdiN2FiODlkLTVmMDctNGY4MC1iNTQ5LTM1MzAwMGJiNDkyZCIsInRpZCI6IjQxZmYyNmRjLTI1MGYtNGIxMy04OTgxLTczOWJlODYxMGMyMSIsInV0aSI6IklxVW5NYlMwNlUtXzlZOVJKQVdXQUEiLCJ2ZXIiOiIyLjAifQ.NShaNz-pCTp0jLlRp-haPavu0Ho5NezngVJ36X9veqsS-PMY887lO1x2Z6nKa-86K2B7exWcACXnP3-k1UdUjlHEuegELdjVRkzWJ5qmXBC7fBsBm3_Qzr1FYirFfBz3dwY1upr5RHlmKkeD0CEacpB67cvS7Y9QgiGX8eBm1rtMRXy7VodWgjRaFo2rd4VdOKTIi-_7A3hSAsA17OxlkyLARoaGC9EXOQ9jjWtuCY9Y8upafBJPoiDxgTkIzUKIHFdcFJsYvOgyHFNjO7Kf6nEkJfLKlDJuLbQRZJhOmExvq-UTDqea3hipLlL8gxxfRblyX8xHT736aY5cp6eUVg" },
    };
}

async Task<AuthenticationResult> AcquireTokenAsync()
{
    var scopes = _configuration.GetSection("Scopes").Get<string[]>();
    var account = (await _publicClientApplication.GetAccountsAsync()).FirstOrDefault();
    try
    {
        return await _publicClientApplication.AcquireTokenSilent(scopes, account)
            .ExecuteAsync();
    }
    catch (MsalUiRequiredException ex)
    {
        Console.WriteLine(ex.Message);       
        try
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var signInTask = _publicClientApplication.AcquireTokenInteractive(scopes)
                    .WithSystemWebViewOptions(new SystemWebViewOptions
                    {
                        OpenBrowserAsync = SystemWebViewOptions.OpenWithChromeEdgeBrowserAsync,
                    })
                    .ExecuteAsync(cancellationTokenSource.Token);
                if (signInTask != await Task.WhenAny(signInTask, Task.Delay(TimeSpan.FromMinutes(2))))
                {
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Timeout.");
                    return null;
                }

                return await signInTask;
            }
        }
        catch (MsalException msalEx)
        {
            Console.WriteLine(msalEx);
            return null;
        }
    }
}
