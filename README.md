# MojangSharp

MojangSharp is a C# wrapper for the [Mojang API](http://wiki.vg/Mojang_API) and [Mojang Authentication API](http://wiki.vg/Authentication).

# Features

- Asynchronous API
- All error and response types handled
- Really easy to use

## Getting started

[![GitHub release](https://img.shields.io/github/release/hawezo/MojangSharp.svg?style=flat-square)](https://github.com/hawezo/MojangSharp/releases)
[![GitHub issues](https://img.shields.io/github/issues/hawezo/MojangSharp.svg?style=flat-square)](https://github.com/hawezo/MojangSharp/issues)
&nbsp;
[![NuGet](https://img.shields.io/nuget/v/Hawezo.MojangSharp.svg?style=flat-square)](https://www.nuget.org/packages/Hawezo.MojangSharp)
[![NuGet downloads](https://img.shields.io/nuget/dt/Hawezo.MojangSharp.svg?style=flat-square)](https://www.nuget.org/packages/Hawezo.MojangSharp)

## Installation

You will need to install MojangSharp by [downloading it](https://github.com/hawezo/MojangSharp/archive/master.zip) or [installing it from NuGet](https://www.nuget.org/packages/Hawezo.MojangSharp) with `MS> Install-Package Hawezo.MojangSharp`.

## Usage

MojangSharp contains a `Endpoints` namespace which contains all of the possible actions. See the few examples below to understand their utilization:

### ApiStatus

First, get `Response` object corresponding to the `Endpoint` you are using. In the case of `ApiStatus`, the `Response` object is `ApiStatusResponse`.
Then, instantiate the `Endpoint` object and call its method asynchronous `PerformRequest()`.

```csharp
ApiStatusResponse status = await new ApiStatus().PerformRequest();
```

If the request is a success, the boolean value of `status.IsSuccess` would be set to `true`. Otherwise, the `Error` property will indicates where is the issue coming from.

Assuming the request is a success, you can access each property of `status` to get the responses you needed:

```csharp
Console.WriteLine($"Mojang: {status.Mojang}");
Console.WriteLine($"Minecraft: {status.Minecraft}");
Console.WriteLine($"Skins: {status.Skins}");
Console.WriteLine($"Sessions: {status.Sessions}");
//...
```


### Authentication

Authentication's request type is the same as the other. You will need to instanciate a `Credentials` object containing the username and the password of the player you want to authenticate.
Then, you will be able to perform the request and get an access token.

```csharp
AuthenticateResponse auth = await new Authenticate(new Credentials() { Username = "<mail>/<username>", Password = "<password>" }).PerformRequest();
if (auth.IsSuccess) {
  Console.WriteLine($"AccessToken: {auth.AccessToken}");
  Console.WriteLine($"ClientToken: {auth.ClientToken}");
} else { // Handle your error }
```

Note that `ClientToken` is an auto-generated token coming from the library. The first time it is used, you can decide to store it somewhere and thus be able to user the `Validate`, `Invalidate` and the other endpoints of the Authentication API.
You can check after an authentication request if the Client Token is the same as the one stored in `Requester.ClientToken`. If not, there is probably an issue with your authentication structure.


### Skins

**Warning** - Please perform your own tests for all skin-related endpoints, this feature has not been tested (but the requests work so it is likely working).

You can change or reset a skin with MojangSharp. To change a skin, you can either call `UploadSkin` endpoint to upload a skin to the Mojang's servers, or call `ChangeSkin` with an URL to the skin you want to change to.

```csharp
Response skinUpload = await new UploadSkin(auth.AccessToken, auth.SelectedProfile.Value, new FileInfo(@"<path>")).PerformRequest();
if (skinUpload.IsSuccess) {
  Console.WriteLine("Successfully changed skin.")
} else { // Handle your errors }
```

### Blocked servers

Mojang has a list of actually blocked addresses, which are SHA1-hashed. Some of them has been cracked by the community and are listed in MojangSharp.

```csharp
BlockedServersResponse servers = await new BlockedServers().PerformRequest();
if (servers.IsSuccess) {
    Console.WriteLine($"{servers.BlockedServers.Count} blocked servers");
    Console.WriteLine($"{servers.BlockedServers.FindAll(x => x.Cracked).Count} cracked");
}
else { // You know what }
```

### Statistics

Mojang offers an endpoint to get its statistics. Although there is not a lot of interest, it can somehow be useful.
You can combine up to 4 statistics in the `Statistics` constructor, in which case the resulting numbers will be added to each other.

```csharp
StatisticsResponse stats = await new Statistics(Item.MinecraftAccountsSold).PerformRequest();
if (stats.IsSuccess) {
    Console.WriteLine($"Total Minecraft accounts sold: {stats.Total}");
    Console.WriteLine($"Last 24h: {stats.Last24h}");
    Console.WriteLine($"Average sell/s: {stats.SaleVelocity}");
} else { // Handle your errors }
```

# Dependencies

MojangSharp uses [Newtonsoft's JSON](https://github.com/JamesNK/Newtonsoft.Json) to parse Mojang's API responses.

# To-do

- [ ] Better error handling?
- [ ] Add a [request limiter](http://wiki.vg/Mojang_API#Notes).


