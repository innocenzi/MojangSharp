using MojangSharp.Api;
using MojangSharp.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using static MojangSharp.Responses.ProfileResponse;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Profile request class
    /// </summary>
    public class Profile : IEndpoint<ProfileResponse>
    {
        // TODO RATE LIMIT

        /// <summary>
        /// Applies unsigned setting to the request
        /// </summary>
        public bool Unsigned { get; private set; }

        /// <summary>
        /// Returns player's username and additional informations
        /// </summary>
        /// <param name="uuid">Player UUID</param>
        /// <param name="unsigned"></param>
        public Profile(string uuid, bool unsigned = true)
        {
            this.Unsigned = unsigned;

            if (this.Unsigned)
                this.Address = new Uri($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}");
            else
                this.Address = new Uri($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}?unsigned=false");
            this.Arguments.Add(uuid);
            this.Arguments.Add(unsigned.ToString());
        }

        /// <summary>
        /// Performs the profile request.
        /// </summary>
        public async override Task<ProfileResponse> PerformRequestAsync()
        {
            this.Response = await Requester.Get(this);

            if (this.Response.IsSuccess)
            {
                JObject profile = JObject.Parse(this.Response.RawMessage);

                return new ProfileResponse(this.Response)
                {
                    Uuid = new Uuid()
                    {
                        PlayerName = profile["name"].ToObject<string>(),
                        Value = profile["id"].ToObject<string>(),
                        Legacy = null,
                        Demo = null,
                    },
                    Properties = new ProfileProperties(profile["properties"].ToObject<JArray>()[0]["value"].ToObject<string>())
                };
            }
            else
            {
                if (this.Response.Code == (HttpStatusCode)429)
                {
                    ProfileResponseError error = new ProfileResponseError(JObject.Parse(this.Response.RawMessage));
                    return new ProfileResponse(this.Response) { Error = error };
                }
                else
                    return new ProfileResponse(Error.GetError(this.Response));
            }
        }
    }
}