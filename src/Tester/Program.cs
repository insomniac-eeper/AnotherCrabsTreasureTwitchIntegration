// See https://aka.ms/new-console-template for more information

using Common.TwitchLibrary.Services;
using DotEnv.Generated;

string dcfRequestQuery;
var twitchId = SecretsEnvironment.TwitchClientId;
AuthService auth = new(twitchId);
var dcfRequest = auth.RequestDeviceCode().GetAwaiter().GetResult();

if (dcfRequest is null)
{
    Console.WriteLine("Failed to get device code");
    return -1;
}
System.Diagnostics.Process.Start(dcfRequest.verification_uri);
Console.ReadKey();

var oauthRequest = auth.RequestAuthCode(dcfRequest?.device_code ?? string.Empty).GetAwaiter().GetResult();
if (oauthRequest is null)
{
    Console.WriteLine("Failed to get oauth token");
    return -1;
}
Console.WriteLine(oauthRequest.access_token);

var api = new TwitchLib.Api.TwitchAPI();
api.Settings.Secret = oauthRequest.access_token;
api.Settings.ClientId = twitchId;

var resu = api.Helix.Users.GetUsersAsync(accessToken: oauthRequest.access_token).GetAwaiter().GetResult();
foreach (var user in resu.Users)
{
    Console.WriteLine(user.DisplayName);
}
return 1;
