using MojangSharp.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Changes a player's skin from an url
    /// </summary>
    public class ChangeSkin : IEndpoint<Response>
    {
        /// <summary>
        /// Creates a change skin request with a given UUID.
        /// </summary>
        /// <param name="accessToken">User Access Token</param>
        /// <param name="uuid">UUID of the player.</param>
        /// <param name="skinUrl">URL of the skin.</param>
        /// <param name="slim">Defines if slim model is used.</param>
        public ChangeSkin(string accessToken, string uuid, string skinUrl, bool slim = false)
        {
            this.Address = new Uri($"https://api.mojang.com/user/profile/{uuid}/skin");
            this.Arguments.Add(accessToken);
            this.Arguments.Add(skinUrl);
            this.Arguments.Add(slim.ToString());
        }

        /// <summary>
        /// Performs the skin change.
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.Response = await Requester.Post(this, new Dictionary<string, string>() {
                { "model", (bool.Parse(Arguments[2]) == true ? "slim" : null) },
                { "url", Arguments[1] },
            });

            if (this.Response.Code == HttpStatusCode.NoContent || this.Response.IsSuccess)
                return new Response(this.Response) { IsSuccess = true };
            else
                return new Response(this.Response);
        }
    }
}