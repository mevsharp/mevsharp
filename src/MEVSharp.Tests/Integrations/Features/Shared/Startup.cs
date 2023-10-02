//using MEVSharp.Features.BuilderAPI.Model;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json.Linq;
//using System.Text.RegularExpressions;

//public class Startupp
//{
//    public void ConfigureServices(IServiceCollection services)
//    {
//        services.AddRouting();
//    }

//    public void Configure(IApplicationBuilder app)
//    {
//        app.UseRouting();
//        app.UseEndpoints(endpoints =>
//        {
//            endpoints.MapGet("/eth/v1/builder/status", async context =>
//            {
//                await context.Response.WriteAsJsonAsync(new
//                {
//                    code = 200,
//                    message = "OK"
//                });
//            });

//            endpoints.MapPost("/eth/v1/builder/validators", async context =>
//            {
//                string jsonString = await new StreamReader(context.Request.Body).ReadToEndAsync();
//                List<RegisterValidatorRequestInner>? requestList = null;

//                try
//                {
//                    requestList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegisterValidatorRequestInner>>(jsonString);

//                    bool isValidRequest = requestList != null && requestList.All(x =>
//                        x.Message != null &&
//                        x.Message.FeeRecipient != null &&
//                        x.Message.GasLimit != null &&
//                        x.Message.Timestamp != null &&
//                        x.Message.Pubkey != null &&
//                        x.Signature != null
//                    );

//                    if (isValidRequest)
//                    {
//                        context.Response.StatusCode = StatusCodes.Status200OK;
//                    }
//                    else
//                    {
//                        Console.WriteLine("Invalid request format");
//                    }
//                }
//                catch
//                {
//                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
//                    string errorMessage;
//                    if (requestList == null)
//                    {
//                        errorMessage = "RequestList is null";
//                    }
//                    else
//                    {
//                        errorMessage = "Invalid request format: " + jsonString;
//                    }
//                    await context.Response.WriteAsJsonAsync(new
//                    {
//                        code = 400,
//                        message = errorMessage
//                    });
//                }
//            });

//            endpoints.MapPost("/eth/v1/builder/blinded_blocks", async context =>
//            {
//                string jsonString = await new StreamReader(context.Request.Body).ReadToEndAsync();

//                try
//                {
//                    var signedBlindedBeaconBlock = Newtonsoft.Json.JsonConvert.DeserializeObject<CapellaSignedBlindedBeaconBlock>(jsonString);

//                    bool isValidRequest = IsJsonObjectValid(jsonString);

//                    if (isValidRequest)
//                    {
//                        string jsonResponse = await File.ReadAllTextAsync("Assets/blinded_blocks_response.json");
//                        Assert.False(string.IsNullOrWhiteSpace(jsonResponse));

//                        context.Response.ContentType = "application/json";
//                        await context.Response.WriteAsync(jsonResponse);
//                    }
//                    else
//                    {
//                        throw new Exception("Invalid request format");
//                    }
//                }
//                catch
//                {
//                    context.Response.StatusCode = 400;
//                    var errorResponse = new
//                    {
//                        code = 400,
//                        message = "Invalid block: missing signature"
//                    };
//                    context.Response.ContentType = "application/json";
//                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse));
//                }
//            });

//            endpoints.MapGet("/eth/v1/builder/header/{slot}/{parent_hash}/{pubkey}", async context =>
//            {
//                var slot = context.Request.RouteValues["slot"] as string;
//                var parentHash = context.Request.RouteValues["parent_hash"] as string;
//                var pubkey = context.Request.RouteValues["pubkey"] as string;

//                bool IsValidSlot(string? slot)
//                {
//                    if (string.IsNullOrEmpty(slot))
//                        return false;

//                    if (!ulong.TryParse(slot, out ulong slotValue))
//                        return false;

//                    return true;
//                }

//                bool IsValidParentHash(string? parentHash)
//                {
//                    if (string.IsNullOrEmpty(parentHash))
//                        return false;

//                    if (!parentHash.StartsWith("0x"))
//                        return false;

//                    return Regex.IsMatch(parentHash.Substring(2), "^[a-fA-F0-9]+$");
//                }

//                bool IsValidPubkey(string? pubkey)
//                {
//                    if (string.IsNullOrEmpty(pubkey))
//                        return false;

//                    if (!pubkey.StartsWith("0x"))
//                        return false;

//                    return Regex.IsMatch(pubkey.Substring(2), "^[a-fA-F0-9]+$");
//                }

//                bool isValid = IsValidSlot(slot) && IsValidParentHash(parentHash) && IsValidPubkey(pubkey);

//                if (isValid)
//                {
//                    string jsonResponse = await File.ReadAllTextAsync("Assets/get_header_response.json");
//                    Assert.False(string.IsNullOrWhiteSpace(jsonResponse));
//                    GetHeaderResponse getHeaderResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GetHeaderResponse>(jsonResponse);
//                    Random random = new Random();
//                    int randomNumber = random.Next(1, 11);
//                    getHeaderResponse.Data.GetGetHeaderResponseDataOneOf1().Message.Value = randomNumber.ToString();

//                    context.Response.ContentType = "application/json";
//                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(getHeaderResponse));
//                }
//                else
//                {
//                    context.Response.StatusCode = 400;
//                    var errorResponse = new
//                    {
//                        code = 400,
//                        message = "Unknown hash: missing parent hash"
//                    };
//                    context.Response.ContentType = "application/json";
//                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse));
//                }
//            });
//        });
//    }
//    private static bool IsJsonObjectValid(JToken token)
//    {
//        switch (token.Type)
//        {
//            case JTokenType.String:
//                return !string.IsNullOrWhiteSpace(token.Value<string>());
//            case JTokenType.Array:
//                return token.Children().All(IsJsonObjectValid);
//            case JTokenType.Object:
//                return token.Children<JProperty>().All(property => IsJsonObjectValid(property.Value));
//            default:
//                return true;
//        }
//    }
//}
