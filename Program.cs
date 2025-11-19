using Cheeseman.Models.TPL;
using Cheeseman.Models.TPL.Quote;
using Cheeseman.Models.TPL.Track;
using Cheeseman.Models.TPL.Transit;
using RestSharp;
using System.Text.Json;

Console.Title = "Cheeseman Third-Party Logistics API Demo";

var client = new RestClient("https://tpl.cheeseman.com");


#region Authentication
#region v1/Authenticate
Console.WriteLine("v1/Authenticate");

Console.Write("Username: ");
string username = Console.ReadLine();

Console.Write("Password: ");
string password = Console.ReadLine();

var authRequest = new RestRequest("/v1/Authenticate", Method.Post)
        .AddParameter("username", username, true)
        .AddParameter("password", password, true);

var authResponse = await client.ExecutePostAsync(authRequest);

var auth = JsonSerializer.Deserialize<ApiResultModel<AuthModel>>(authResponse.Content);

// Check for Errors
if (auth.Errors?.Length > 0)
    foreach (var e in auth.Errors)
        Console.WriteLine(e);

// Check for Message
if (!string.IsNullOrWhiteSpace(auth.Result?.MESSAGE))
    Console.WriteLine(auth.Result.MESSAGE);

// Add Authorization Bearer Token to client for all subsequent API requests.
if (!string.IsNullOrWhiteSpace(auth.Result?.TOKEN))
{
    Console.WriteLine($"Authorization Bearer Token: {auth.Result.TOKEN}");
    client.AddDefaultHeader("Authorization", $"bearer {auth.Result.TOKEN}");
}

Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion v1/Authenticate
#endregion Authentication

#region Quoting
#region v1/Quote/LTL
Console.WriteLine("v1/Quote/LTL");

var ltlQuoteRequestModel = new LTLQuoteRequestModel()
{
    REFERENCE = "MY DEMO REFERENCE",
    START_ZONE = "45846",
    PICK_UP_APPT_REQ = false,
    END_ZONE = "21703",
    DELIVERY_APPT_REQ = false,
    DETAILS = new List<LTLQuoteRequestDetailModel>()
    {
        new LTLQuoteRequestDetailModel()
        {
            COMMODITY = "55",
            DESCRIPTION = "FREIGHT DESCRIPTION FOR DEMONSTRATION PURPSOSES ONLY",
            PIECES = 1,
            WEIGHT = 1500,
            DANGEROUS_GOODS = false,
            TEMP_CONTROLLED = false,
            STACKABLE = false
        }
    }
};

var ltlQuoteRequest = new RestRequest("/v1/Quote/LTL", Method.Post)
    .AddBody(ltlQuoteRequestModel);

var ltlQuoteResponse = await client.ExecutePostAsync(ltlQuoteRequest);

var ltlQuote = JsonSerializer.Deserialize<ApiResultModel<QuoteModel>>(ltlQuoteResponse.Content);

// Check for Errors
if (ltlQuote.Errors?.Length > 0)
    foreach (var e in ltlQuote.Errors)
        Console.WriteLine(e);

// Display Quote Number and Total Charges
if (!string.IsNullOrWhiteSpace(ltlQuote.Result?.BILL_NUMBER))
    Console.WriteLine($"{ltlQuote.Result.BILL_NUMBER} {ltlQuote.Result.TOTAL_CHARGES:c2}");

Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion v1/Quote/LTL

#region v1/Quote/Active
Console.WriteLine("v1/Quote/Active");

var activeQuotesRequest = new RestRequest("/v1/Quote/Active", Method.Get);

var activeQuotesResponse = await client.ExecuteGetAsync(activeQuotesRequest);

var activeQuotes = JsonSerializer.Deserialize<ApiResultModel<List<QuoteModel>>>(activeQuotesResponse.Content);

// Check for Errors
if (activeQuotes.Errors?.Length > 0)
    foreach (var e in activeQuotes.Errors)
        Console.WriteLine(e);

// Display Active Quotes
if (activeQuotes.Result != null)
    foreach (var q in activeQuotes.Result)
        Console.WriteLine($"{q.BILL_NUMBER} {q.TOTAL_CHARGES:c2}");

Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion v1/Quote/Active

#region v1/Quote/View/{quoteNumber}
Console.WriteLine("v1/Quote/View/{quoteNumber}");

var viewQuoteRequest = new RestRequest($"/v1/Quote/View/{ltlQuote.Result.BILL_NUMBER}", Method.Get);

var viewQuoteResponse = await client.ExecuteGetAsync(viewQuoteRequest);

var viewQuote = JsonSerializer.Deserialize<ApiResultModel<QuoteModel>>(viewQuoteResponse.Content);

// Check for Errors
if (viewQuote.Errors?.Length > 0)
    foreach (var e in viewQuote.Errors)
        Console.WriteLine(e);

// Display Quote Number and Total Charges
if (!string.IsNullOrWhiteSpace(viewQuote.Result?.BILL_NUMBER))
    Console.WriteLine($"{viewQuote.Result.BILL_NUMBER} {viewQuote.Result.TOTAL_CHARGES:c2}");

Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion v1/Quote/View/{quoteNumber}
#endregion Quoting

#region Tracking
#region v1/Track/{trackingNumber}
Console.WriteLine("v1/Track/{trackingNumber}");

string trackingNumber = "MyTrackingNumber";
var trackRequest = new RestRequest($"/v1/Track/{trackingNumber}", Method.Get);

var trackResponse = await client.ExecuteGetAsync(trackRequest);

var track = JsonSerializer.Deserialize<ApiResultModel<TrackModel>>(trackResponse.Content);

// Check for Errors
if (track.Errors?.Length > 0)
    foreach (var e in track.Errors)
        Console.WriteLine(e);

// Display Bill Number, Current Status, and Current Location
if (!string.IsNullOrWhiteSpace(track.Result?.BILL_NUMBER))
    Console.WriteLine($"{track.Result.BILL_NUMBER} {track.Result.CURRENT_STATUS} {track.Result.CURRENT_ZONE} {track.Result.CURRENT_ZONE_DESC}");

Console.WriteLine("v1/Track/{trackingNumber}");
Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion v1/Track/{trackingNumber}
#endregion Tracking

#region Transit
Console.WriteLine("v1/Transit");

var transitTimeRequestModel = new TransitTimeRequestModel()
{
    SERVICE_LEVEL = "LTL",
    START_ZONE = "45846",
    END_ZONE = "21703",
    PICK_UP_DATE = DateTime.Now
};

var transitTimeRequest = new RestRequest("/v1/Transit", Method.Post)
    .AddBody(transitTimeRequestModel);

var transitTimeResponse = await client.ExecutePostAsync(transitTimeRequest);

var transitTime = JsonSerializer.Deserialize<ApiResultModel<TransitTimeModel>>(transitTimeResponse.Content);

// Check for Errors
if (transitTime.Errors?.Length > 0)
    foreach (var e in transitTime.Errors)
        Console.WriteLine(e);

Console.WriteLine($"Standard Service Days:{transitTime.Result.STANDARD_TRANSIT_DAYS:n0}, Delays:{transitTime.Result.DELAYS:n0} Estimated Delivery Date:{transitTime.Result.ESTIMATED_DELIVERY_DATE:MM/dd/yyyy}");

Console.WriteLine("Press any key to continue...");
Console.ReadLine();
Console.Clear();
#endregion Transit