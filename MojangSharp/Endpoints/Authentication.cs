using MojangSharp.Api;
using MojangSharp.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static MojangSharp.Responses.AuthenticateResponse;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Represents a couple of username and password for authentication purposes
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Authenticate request class
    /// </summary>
    public class Authenticate : IEndpoint<AuthenticateResponse>
    {
        /// <summary>
        /// Sends a request of authentication
        /// </summary>
        public Authenticate(Credentials credentials)
        {
            this.Address = new Uri($"https://authserver.mojang.com/authenticate");
            this.Arguments.Add(credentials.Username);
            this.Arguments.Add(credentials.Password);
        }

        /// <summary>
        /// Performs the authentication.
        /// </summary>
        public async override Task<AuthenticateResponse> PerformRequestAsync()
        {
            this.PostContent = new JObject(
                                    new JProperty("agent",
                                        new JObject(
                                            new JProperty("name", "Minecraft"),
                                            new JProperty("version", "1"))),
                                    new JProperty("username", this.Arguments[0]),
                                    new JProperty("password", this.Arguments[1]),
                                    new JProperty("clientToken", Requester.ClientToken),
                                    new JProperty("requestUser", true)).ToString();

            this.Response = await Requester.Post(this);
            if (this.Response.IsSuccess)
            {
                JObject user = JObject.Parse(this.Response.RawMessage);
                List<Uuid> availableProfiles = new List<Uuid>();

                foreach (JObject profile in user["availableProfiles"])
                    availableProfiles.Add(new Uuid()
                    {
                        PlayerName = profile["name"].ToObject<string>(),
                        Value = profile["id"].ToObject<string>(),
                        Legacy = (profile.ToString().Contains("legacyProfile") ? profile["legacyProfile"].ToObject<bool>() : false),
                        Demo = null
                    });

                return new AuthenticateResponse(this.Response)
                {
                    AccessToken = user["accessToken"].ToObject<string>(),
                    ClientToken = user["clientToken"].ToObject<string>(),
                    AvailableProfiles = availableProfiles,
                    SelectedProfile = new Uuid()
                    {
                        PlayerName = user["selectedProfile"]["name"].ToObject<string>(),
                        Value = user["selectedProfile"]["id"].ToObject<string>(),
                        Legacy = (user["selectedProfile"].ToString().Contains("legacyProfile") ? user["selectedProfile"]["legacyProfile"].ToObject<bool>() : false),
                        Demo = null
                    },
                    User = user["user"].ToObject<UserData>()
                };
            }
            else
            {
                try
                {
                    AuthenticationResponseError error = new AuthenticationResponseError(JObject.Parse(this.Response.RawMessage));
                    return new AuthenticateResponse(this.Response) { Error = error };
                }
                catch (Exception)
                {
                    return new AuthenticateResponse(Error.GetError(this.Response));
                }
            }
        }
    }

    /// <summary>
    /// Refresh authentication request class
    /// </summary>
    public class Refresh : IEndpoint<TokenResponse>
    {
        /// <summary>
        /// Refreshes the access token. Must be the same instance as authenticate.
        /// </summary>
        public Refresh(string accessToken)
        {
            this.Address = new Uri($"https://authserver.mojang.com/refresh");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs refresh token.
        /// </summary>
        /// <returns></returns>
        public async override Task<TokenResponse> PerformRequestAsync()
        {
            this.PostContent = new JObject(
                                    new JProperty("accessToken", this.Arguments[0]),
                                    new JProperty("clientToken", Requester.ClientToken)).ToString();

            this.Response = await Requester.Post(this);

            if (this.Response.IsSuccess)
            {
                JObject refresh = JObject.Parse(this.Response.RawMessage);

                return new TokenResponse(this.Response)
                {
                    AccessToken = refresh["accessToken"].ToObject<string>()
                };
            }
            else
                return new TokenResponse(Error.GetError(this.Response));
        }
    }

    /// <summary>
    /// Validate token request class
    /// </summary>
    public class Validate : IEndpoint<Response>
    {
        /// <summary>
        /// Refreshes the access token. Must be the same instance as authenticate.
        /// </summary>
        public Validate(string accessToken)
        {
            this.Address = new Uri($"https://authserver.mojang.com/validate");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs validate token.
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.PostContent = new JObject(
                                    new JProperty("accessToken", this.Arguments[0]),
                                    new JProperty("clientToken", Requester.ClientToken)).ToString();

            this.Response = await Requester.Post(this);

            if (this.Response.Code == HttpStatusCode.NoContent)
                return new Response(this.Response) { IsSuccess = true };
            else
                return new Response(Error.GetError(this.Response));
        }
    }

    /// <summary>
    /// Sign out request class
    /// </summary>
    public class Signout : IEndpoint<Response>
    {
        /// <summary>
        /// Refreshes the access token. Must be the same instance as authenticate.
        /// </summary>
        public Signout(Credentials credentials)
        {
            this.Address = new Uri($"https://authserver.mojang.com/signout");
            this.Arguments.Add(credentials.Username);
            this.Arguments.Add(credentials.Password);
        }

        /// <summary>
        /// Performs signing out
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.PostContent = new JObject(
                                    new JProperty("username", this.Arguments[0]),
                                    new JProperty("password", this.Arguments[1])).ToString();

            this.Response = await Requester.Post(this);

            if (string.IsNullOrWhiteSpace(this.Response.RawMessage))
                return new Response(this.Response) { IsSuccess = true };
            else
                return new Response(Error.GetError(this.Response));
        }
    }

    /// <summary>
    /// Invalidate token request class
    /// </summary>
    public class Invalidate : IEndpoint<Response>
    {
        /// <summary>
        /// Refreshes the access token. Must be the same instance as authenticate.
        /// </summary>
        public Invalidate(string accessToken)
        {
            this.Address = new Uri($"https://authserver.mojang.com/invalidate");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs validate token.
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.PostContent = new JObject(
                                    new JProperty("accessToken", this.Arguments[0]),
                                    new JProperty("clientToken", Requester.ClientToken)).ToString();

            this.Response = await Requester.Post(this);

            if (this.Response.Code == HttpStatusCode.NoContent)
            {
                return new Response(this.Response) { IsSuccess = true }; ;
            }
            else
                return new Response(Error.GetError(this.Response));
        }
    }
}