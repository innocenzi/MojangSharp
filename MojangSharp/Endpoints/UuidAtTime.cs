using MojangSharp.Api;
using MojangSharp.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MojangSharp.Endpoints
{

    public class UuidAtTime : IEndpoint<UuidAtTimeResponse>
    {

        /// <summary>
        /// Instantiates the endpoints which allows to get a player's UUID at a certain time.
        /// <paramref name="username">Username of the player you want to get UUID's</paramref>
        /// <paramref name="date">Date at which you want to get the UUID</paramref>
        /// </summary>
        public UuidAtTime(string username, DateTime date)
        {
            int timespan = (Int32)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            this.Address = new Uri($"https://api.mojang.com/users/profiles/minecraft/{username}?at={timespan}");
            this.Arguments.Add(username);
            this.Arguments.Add(timespan.ToString());
        }
        
        public async override Task<UuidAtTimeResponse> PerformRequest()
        {
            this.Response = await Requester.Get(this);

            if (this.Response.IsSuccess)
            {
                JObject uuid = JObject.Parse(this.Response.RawMessage);

                return new UuidAtTimeResponse(this.Response)
                {
                    Uuid = new Uuid()
                    {
                        PlayerName = uuid["name"].ToObject<string>(),
                        Value = uuid["id"].ToObject<string>(),

                        // Kind of ugly, could be better, 6/10
                        Legacy = (this.Response.RawMessage.Contains("legacy") ? uuid["legacy"].ToObject<bool>() : false),
                        Demo = (this.Response.RawMessage.Contains("demo") ? uuid["demo"].ToObject<bool>() : false),
                    },
                    Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(this.Arguments[1])).ToLocalTime()
                };
            }
            else
                return new UuidAtTimeResponse(Error.GetError(this.Response));
        }
    }

}
